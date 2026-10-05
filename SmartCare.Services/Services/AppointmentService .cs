using System.Data;
using AutoMapper;
using SmartCare.Core.Entities.Appointments;
using SmartCare.Core.Entities.Doctors;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Specifications.Appointments;
using SmartCare.Services.Abstraction.DTOs.Appointments;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Responses;

namespace SmartCare.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ITimeSlotService _timeSlotService;

    public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, ITimeSlotService timeSlotService) { _unitOfWork = unitOfWork; _mapper = mapper; _timeSlotService = timeSlotService; }

    public async Task<ApiResponse<AppointmentDto>> CreateAsync(CreateAppointmentDto dto, Guid patientId)
    {
        ValidateTimeRange(dto.StartTime, dto.EndTime);
        if (dto.AppointmentDate < DateOnly.FromDateTime(DateTime.Now))
            return ApiResponse<AppointmentDto>.Fail("Appointment date cannot be in the past.");
        if (!string.IsNullOrWhiteSpace(dto.Reason) && dto.Reason.Length > 500)
            return ApiResponse<AppointmentDto>.Fail("Reason cannot exceed 500 characters.");
        var doctorRepository = _unitOfWork.GetRepository<Doctor, Guid>();
        var doctor = await doctorRepository.GetByIdAsync(dto.DoctorId);
        if (doctor is null) 
            return ApiResponse<AppointmentDto>.Fail("Doctor not found.");
        if (!doctor.IsActive)
            return ApiResponse<AppointmentDto>.Fail("Doctor is not available.");
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var overlappingRepository = _unitOfWork.GetRepository<Appointment, Guid>();
            var overlapSpec = new AppointmentOverlappingSpec(dto.DoctorId, dto.AppointmentDate, dto.StartTime, dto.EndTime);
            var overlappingAppointments = await overlappingRepository.GetBySpecificationAsync(overlapSpec);
            if (overlappingAppointments.Any()) { await _unitOfWork.RollbackAsync(); 
                return ApiResponse<AppointmentDto>.Fail("The selected time slot is already booked."); }
            var availableSlots = await _timeSlotService.GetAvailableSlotsAsync(dto.DoctorId, dto.AppointmentDate);
            var selectedSlot = availableSlots.FirstOrDefault(s => s.StartTime == dto.StartTime && s.EndTime == dto.EndTime && s.IsAvailable);
            if (selectedSlot is null)
            { await _unitOfWork.RollbackAsync(); return ApiResponse<AppointmentDto>.Fail("The selected time slot is not available."); }
            var appointment = new Appointment { Id = Guid.NewGuid(), PatientId = patientId, DoctorId = dto.DoctorId, AppointmentDate = dto.AppointmentDate, StartTime = dto.StartTime, EndTime = dto.EndTime, Reason = dto.Reason, ConsultationFee = doctor.ConsultationFee, Status = AppointmentStatus.Pending };
            await overlappingRepository.AddAsync(appointment);
            await _unitOfWork.CommitAsync();
            var result = _mapper.Map<AppointmentDto>(appointment);
            return ApiResponse<AppointmentDto>.Ok(result, "Appointment created successfully.");
        }
        catch { await _unitOfWork.RollbackAsync(); throw; }
    }

    public async Task<ApiResponse<AppointmentDetailsDto>> GetByIdAsync(Guid appointmentId, Guid currentUserId, string currentUserRole)
    {
        var repository = _unitOfWork.GetRepository<Appointment, Guid>();
        var appointmentSpec = new AppointmentWithDetailsSpec(appointmentId);
        var appointments = await repository.GetBySpecificationAsync(appointmentSpec);
        var appointment = appointments.FirstOrDefault();
        if (appointment is null)
            return ApiResponse<AppointmentDetailsDto>.Fail("Appointment not found.");
        if (currentUserRole == "Patient" && appointment.Patient.UserId != currentUserId)
            return ApiResponse<AppointmentDetailsDto>.Fail("You are not authorized to access this appointment.");
        if (currentUserRole == "Doctor" && appointment.Doctor.UserId != currentUserId)
            return ApiResponse<AppointmentDetailsDto>.Fail("You are not authorized to access this appointment.");
        var result = _mapper.Map<AppointmentDetailsDto>(appointment);
        return ApiResponse<AppointmentDetailsDto>.Ok(result, "Appointment retrieved successfully.");
    }

    public async Task<ApiResponse<PaginatedResponse<AppointmentDetailsDto>>> GetMyAppointmentsAsync(Guid patientId, AppointmentSpecParams specParams)
    {
        var repository = _unitOfWork.GetRepository<Appointment, Guid>();
        specParams.PatientId = patientId; // Never trust PatientId coming from the request.
        var specification = new AppointmentFilterPaginatedSpec(specParams);
        var appointments = await repository.GetBySpecificationAsync(specification);
        var totalCount = await repository.CountAsync(specification);
        var items = _mapper.Map<IReadOnlyList<AppointmentDetailsDto>>(appointments);
        var response = new PaginatedResponse<AppointmentDetailsDto>(items, specParams.PageIndex, specParams.PageSize, totalCount);
        return ApiResponse<PaginatedResponse<AppointmentDetailsDto>>.Ok(response, "Appointments retrieved successfully.");
    }

    public async Task<ApiResponse<PaginatedResponse<AppointmentDetailsDto>>> GetAllAsync(AppointmentSpecParams specParams, Guid currentUserId, string currentUserRole)
    {
        var repository = _unitOfWork.GetRepository<Appointment, Guid>();
        if (currentUserRole == "Doctor")
        {
            var doctorRepository = _unitOfWork.GetRepository<Doctor, Guid>();
            var doctors = await doctorRepository.FindAsync(d => d.UserId == currentUserId);
            var doctor = doctors.FirstOrDefault();
            if (doctor is null) return ApiResponse<PaginatedResponse<AppointmentDetailsDto>>.Fail("Doctor profile not found.");
            specParams.DoctorId = doctor.Id; // Doctor can only see his own appointments.
        }
        var specification = new AppointmentFilterPaginatedSpec(specParams);
        var appointments = await repository.GetBySpecificationAsync(specification);
        var totalCount = await repository.CountAsync(specification);
        var items = _mapper.Map<IReadOnlyList<AppointmentDetailsDto>>(appointments);
        var response = new PaginatedResponse<AppointmentDetailsDto>(items, specParams.PageIndex, specParams.PageSize, totalCount);
        return ApiResponse<PaginatedResponse<AppointmentDetailsDto>>.Ok(response, "Appointments retrieved successfully.");
    }

    public async Task<ApiResponse<AppointmentDetailsDto>> CancelAsync(Guid appointmentId, Guid currentUserId, string? cancelReason)
    {
        if (!string.IsNullOrWhiteSpace(cancelReason) && cancelReason.Length > 500)
            return ApiResponse<AppointmentDetailsDto>.Fail("Cancel reason cannot exceed 500 characters.");
        var repository = _unitOfWork.GetRepository<Appointment, Guid>();
        var specification = new AppointmentWithDetailsSpec(appointmentId);
        var appointments = await repository.GetBySpecificationAsync(specification);
        var appointment = appointments.FirstOrDefault();
        if (appointment is null) 
            return ApiResponse<AppointmentDetailsDto>.Fail("Appointment not found.");
        var isPatient = appointment.Patient.UserId == currentUserId;
        var isDoctor = appointment.Doctor.UserId == currentUserId;
        if (!isPatient && !isDoctor)
            return ApiResponse<AppointmentDetailsDto>.Fail("You are not authorized to cancel this appointment.");
        if (appointment.Status == AppointmentStatus.Cancelled)
            return ApiResponse<AppointmentDetailsDto>.Fail("Appointment is already cancelled.");
        if (appointment.Status == AppointmentStatus.Completed) 
            return ApiResponse<AppointmentDetailsDto>.Fail("Completed appointment cannot be cancelled.");
        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancelReason = cancelReason;
        repository.Update(appointment);
        await _unitOfWork.SaveChangesAsync();
        var result = _mapper.Map<AppointmentDetailsDto>(appointment);
        return ApiResponse<AppointmentDetailsDto>.Ok(result, "Appointment cancelled successfully.");
    }

    public async Task<ApiResponse<AppointmentDetailsDto>> RescheduleAsync(Guid appointmentId, Guid patientId, RescheduleAppointmentDto dto)
    {
        ValidateTimeRange(dto.StartTime, dto.EndTime);
        if (dto.AppointmentDate < DateOnly.FromDateTime(DateTime.Now))
            return ApiResponse<AppointmentDetailsDto>.Fail("Appointment date cannot be in the past.");
        var repository = _unitOfWork.GetRepository<Appointment, Guid>();
        var specification = new AppointmentWithDetailsSpec(appointmentId, patientId);
        var appointments = await repository.GetBySpecificationAsync(specification);
        var appointment = appointments.FirstOrDefault();
        if (appointment is null)
            return ApiResponse<AppointmentDetailsDto>.Fail("Appointment not found.");
        if (appointment.Status == AppointmentStatus.Cancelled)
            return ApiResponse<AppointmentDetailsDto>.Fail("Cancelled appointment cannot be rescheduled.");
        if (appointment.Status == AppointmentStatus.Completed) 
            return ApiResponse<AppointmentDetailsDto>.Fail("Completed appointment cannot be rescheduled.");
        await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var overlapSpec = new AppointmentOverlappingSpec(appointment.DoctorId, dto.AppointmentDate, dto.StartTime, dto.EndTime);
            var overlappingAppointments = await repository.GetBySpecificationAsync(overlapSpec);
            var hasConflict = overlappingAppointments.Any(a => a.Id != appointment.Id);
            if (hasConflict) { await _unitOfWork.RollbackAsync(); 
                return ApiResponse<AppointmentDetailsDto>.Fail("The selected time slot is already booked."); }
            var availableSlots = await _timeSlotService.GetAvailableSlotsAsync(appointment.DoctorId, dto.AppointmentDate);
            var selectedSlot = availableSlots.FirstOrDefault(s => s.StartTime == dto.StartTime && s.EndTime == dto.EndTime && s.IsAvailable);
            if (selectedSlot is null) { await _unitOfWork.RollbackAsync();
                return ApiResponse<AppointmentDetailsDto>.Fail("The selected time slot is not available."); }
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.StartTime = dto.StartTime;
            appointment.EndTime = dto.EndTime;
            repository.Update(appointment);
            await _unitOfWork.CommitAsync();
            var result = _mapper.Map<AppointmentDetailsDto>(appointment);
            return ApiResponse<AppointmentDetailsDto>.Ok(result, "Appointment rescheduled successfully.");
        }
        catch { await _unitOfWork.RollbackAsync(); throw; }
    }

    public async Task<ApiResponse<AppointmentDetailsDto>> ConfirmAsync(Guid appointmentId, Guid doctorUserId)
    {
        var repository = _unitOfWork.GetRepository<Appointment, Guid>();
        var specification = new AppointmentWithDetailsSpec(appointmentId);
        var appointments = await repository.GetBySpecificationAsync(specification);
        var appointment = appointments.FirstOrDefault();
        if (appointment is null)
            return ApiResponse<AppointmentDetailsDto>.Fail("Appointment not found.");
        if (appointment.Doctor.UserId != doctorUserId) 
            return ApiResponse<AppointmentDetailsDto>.Fail("You are not authorized to confirm this appointment.");
        if (appointment.Status != AppointmentStatus.Pending)
            return ApiResponse<AppointmentDetailsDto>.Fail("Only pending appointments can be confirmed.");
        appointment.Status = AppointmentStatus.Confirmed;
        repository.Update(appointment);
        await _unitOfWork.SaveChangesAsync();
        var result = _mapper.Map<AppointmentDetailsDto>(appointment);
        return ApiResponse<AppointmentDetailsDto>.Ok(result, "Appointment confirmed successfully.");
    }

    public async Task<ApiResponse<AppointmentDetailsDto>> CompleteAsync(Guid appointmentId, Guid doctorUserId)
    {
        var repository = _unitOfWork.GetRepository<Appointment, Guid>();
        var specification = new AppointmentWithDetailsSpec(appointmentId);
        var appointments = await repository.GetBySpecificationAsync(specification);
        var appointment = appointments.FirstOrDefault();
        if (appointment is null)
            return ApiResponse<AppointmentDetailsDto>.Fail("Appointment not found.");
        if (appointment.Doctor.UserId != doctorUserId)
            return ApiResponse<AppointmentDetailsDto>.Fail("You are not authorized to complete this appointment.");
        if (appointment.Status != AppointmentStatus.Confirmed) 
            return ApiResponse<AppointmentDetailsDto>.Fail("Only confirmed appointments can be completed.");
        appointment.Status = AppointmentStatus.Completed;
        repository.Update(appointment);
        await _unitOfWork.SaveChangesAsync();
        var result = _mapper.Map<AppointmentDetailsDto>(appointment);
        return ApiResponse<AppointmentDetailsDto>.Ok(result, "Appointment completed successfully.");
    }

    private static void ValidateTimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime) throw new ArgumentException("End time must be greater than start time.");
    }
}