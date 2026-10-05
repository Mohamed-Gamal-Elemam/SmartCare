using AutoMapper;
using SmartCare.Core.Entities.DoctorAvailabilitys;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Specifications.DoctorAvailabilitySpecifications;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using SmartCare.Services.Abstraction.Exceptions;
using SmartCare.Services.Abstraction.Services;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Services;

public class DoctorAvailabilityService : IDoctorAvailabilityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DoctorAvailabilityService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // =========================================================
    // GET AVAILABILITIES
    // =========================================================

    public async Task<ApiResponse<IReadOnlyList<DoctorAvailabilityDto>>> GetDoctorAvailabilitiesAsync(Guid doctorId)
    {
        var spec = new DoctorAvailabilityWithBreaksSpec(doctorId);

        var availabilities = await _unitOfWork
            .GetRepository<DoctorAvailability, Guid>()
            .GetBySpecificationAsync(spec);

        var result = _mapper.Map<IReadOnlyList<DoctorAvailabilityDto>>(availabilities);

        return ApiResponse<IReadOnlyList<DoctorAvailabilityDto>>.Ok(result, "Doctor availabilities retrieved successfully.");
    }

    // =========================================================
    // GET EXCEPTIONS
    // =========================================================

    public async Task<ApiResponse<IReadOnlyList<DoctorAvailabilityExceptionDto>>> GetDoctorExceptionsAsync(Guid doctorId)
    {
        var exceptions = await _unitOfWork
            .GetRepository<DoctorAvailabilityException, Guid>()
            .FindAsync(x => x.DoctorId == doctorId);

        var result = _mapper.Map<IReadOnlyList<DoctorAvailabilityExceptionDto>>(exceptions);

        return ApiResponse<IReadOnlyList<DoctorAvailabilityExceptionDto>>.Ok(result, "Doctor availability exceptions retrieved successfully.");
    }

    // =========================================================
    // CREATE AVAILABILITY
    // =========================================================

    public async Task<ApiResponse<DoctorAvailabilityDto>> CreateAvailabilityAsync(Guid doctorId, CreateDoctorAvailabilityDto dto)
    {
        ValidateAvailability(dto.StartTime, dto.EndTime, dto.SlotDurationInMinutes);
        ValidateBreaks(dto.Breaks, dto.StartTime, dto.EndTime);

        // Prevent duplicate schedule for the same doctor/day.
        var spec = new DoctorAvailabilityWithBreaksSpec(doctorId, dto.DayOfWeek);

        var existing = await _unitOfWork
            .GetRepository<DoctorAvailability, Guid>()
            .GetBySpecificationAsync(spec);

        if (existing.Any())
        {
            throw new ConflictException($"An availability schedule already exists for {dto.DayOfWeek}.");
        }

        var availability = _mapper.Map<DoctorAvailability>(dto);
        availability.DoctorId = doctorId;
        availability.IsActive = true;

        var availabilityRepo = _unitOfWork.GetRepository<DoctorAvailability, Guid>();
        var breakRepo = _unitOfWork.GetRepository<DoctorAvailabilityBreak, Guid>();

        await availabilityRepo.AddAsync(availability);

        // Add breaks explicitly.
        foreach (var breakDto in dto.Breaks)
        {
            var breakEntity = _mapper.Map<DoctorAvailabilityBreak>(breakDto);
            breakEntity.DoctorAvailabilityId = availability.Id;

            await breakRepo.AddAsync(breakEntity);
        }

        await _unitOfWork.SaveChangesAsync();

        // Reload with breaks.
        var createdSpec = new DoctorAvailabilityWithBreaksSpec(doctorId, availability.DayOfWeek);
        var createdList = await availabilityRepo.GetBySpecificationAsync(createdSpec);
        var created = createdList.FirstOrDefault(x => x.Id == availability.Id);

        if (created == null)
        {
            throw new NotFoundException("Created availability could not be retrieved.");
        }

        var result = _mapper.Map<DoctorAvailabilityDto>(created);

        return ApiResponse<DoctorAvailabilityDto>.Ok(result, "Doctor availability created successfully.");
    }

    // =========================================================
    // UPDATE AVAILABILITY
    // =========================================================

    public async Task<ApiResponse<DoctorAvailabilityDto>> UpdateAvailabilityAsync(Guid doctorId, Guid availabilityId, UpdateDoctorAvailabilityDto dto)
    {
        ValidateAvailability(dto.StartTime, dto.EndTime, dto.SlotDurationInMinutes);
        ValidateBreaks(dto.Breaks, dto.StartTime, dto.EndTime);

        var availabilityRepo = _unitOfWork.GetRepository<DoctorAvailability, Guid>();
        var breakRepo = _unitOfWork.GetRepository<DoctorAvailabilityBreak, Guid>();

        var spec = new DoctorAvailabilityWithBreaksSpec(doctorId);
        var availabilities = await availabilityRepo.GetBySpecificationAsync(spec);

        var availability = availabilities.FirstOrDefault(x => x.Id == availabilityId);

        if (availability == null)
        {
            throw new NotFoundException("Availability schedule not found or unauthorized.");
        }

        // If changing the day, make sure another schedule doesn't already exist for that day.
        var duplicate = availabilities.FirstOrDefault(x => x.Id != availabilityId && x.DayOfWeek == dto.DayOfWeek);

        if (duplicate != null)
        {
            throw new ConflictException($"An availability schedule already exists for {dto.DayOfWeek}.");
        }

        // Update parent.
        availability.DayOfWeek = dto.DayOfWeek;
        availability.StartTime = dto.StartTime;
        availability.EndTime = dto.EndTime;
        availability.SlotDurationInMinutes = dto.SlotDurationInMinutes;
        availability.IsActive = true;

        availabilityRepo.Update(availability);

        // Delete old breaks.
        var oldBreaks = await breakRepo.FindAsync(x => x.DoctorAvailabilityId == availability.Id);

        foreach (var oldBreak in oldBreaks)
        {
            breakRepo.Delete(oldBreak);
        }

        // Add new breaks.
        foreach (var breakDto in dto.Breaks)
        {
            var breakEntity = _mapper.Map<DoctorAvailabilityBreak>(breakDto);
            breakEntity.DoctorAvailabilityId = availability.Id;

            await breakRepo.AddAsync(breakEntity);
        }

        await _unitOfWork.SaveChangesAsync();

        // Reload with breaks.
        var updatedSpec = new DoctorAvailabilityWithBreaksSpec(doctorId, dto.DayOfWeek);
        var updatedList = await availabilityRepo.GetBySpecificationAsync(updatedSpec);
        var updated = updatedList.FirstOrDefault(x => x.Id == availabilityId);

        if (updated == null)
        {
            throw new NotFoundException("Updated availability could not be retrieved.");
        }

        var result = _mapper.Map<DoctorAvailabilityDto>(updated);

        return ApiResponse<DoctorAvailabilityDto>.Ok(result, "Doctor availability updated successfully.");
    }

    // =========================================================
    // ADD EXCEPTION
    // =========================================================

    public async Task<ApiResponse<DoctorAvailabilityExceptionDto>> AddExceptionAsync(Guid doctorId, CreateDoctorAvailabilityExceptionDto dto)
    {
        var exceptionEntity = _mapper.Map<DoctorAvailabilityException>(dto);
        exceptionEntity.DoctorId = doctorId;

        var repo = _unitOfWork.GetRepository<DoctorAvailabilityException, Guid>();

        await repo.AddAsync(exceptionEntity);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<DoctorAvailabilityExceptionDto>(exceptionEntity);

        return ApiResponse<DoctorAvailabilityExceptionDto>.Ok(result, "Availability exception added successfully.");
    }

    // =========================================================
    // DELETE AVAILABILITY
    // =========================================================

    public async Task<ApiResponse<bool>> DeleteAvailabilityAsync(Guid availabilityId, Guid doctorId)
    {
        var repo = _unitOfWork.GetRepository<DoctorAvailability, Guid>();
        var availability = await repo.GetByIdAsync(availabilityId);

        if (availability == null || availability.DoctorId != doctorId)
        {
            throw new NotFoundException("Availability schedule not found or unauthorized.");
        }

        repo.Delete(availability);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Availability deleted successfully.");
    }

    // =========================================================
    // DELETE EXCEPTION
    // =========================================================

    public async Task<ApiResponse<bool>> DeleteExceptionAsync(Guid exceptionId, Guid doctorId)
    {
        var repo = _unitOfWork.GetRepository<DoctorAvailabilityException, Guid>();
        var exception = await repo.GetByIdAsync(exceptionId);

        if (exception == null || exception.DoctorId != doctorId)
        {
            throw new NotFoundException("Exception record not found or unauthorized.");
        }

        repo.Delete(exception);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Availability exception deleted successfully.");
    }

    // =========================================================
    // VALIDATION
    // =========================================================

    private static void ValidateAvailability(TimeOnly startTime, TimeOnly endTime, int slotDurationInMinutes)
    {
        if (startTime >= endTime)
        {
            throw new BadRequestException("StartTime must be earlier than EndTime.");
        }

        if (slotDurationInMinutes <= 0)
        {
            throw new BadRequestException("SlotDurationInMinutes must be greater than zero.");
        }

        var totalMinutes = (int)(endTime - startTime).TotalMinutes;

        if (slotDurationInMinutes > totalMinutes)
        {
            throw new BadRequestException("Slot duration cannot be greater than the availability duration.");
        }
    }

    private static void ValidateBreaks(IEnumerable<CreateDoctorAvailabilityBreakDto> breaks, TimeOnly availabilityStart, TimeOnly availabilityEnd)
    {
        var breakList = breaks.ToList();

        foreach (var breakItem in breakList)
        {
            if (breakItem.StartTime >= breakItem.EndTime)
            {
                throw new BadRequestException("Break StartTime must be earlier than EndTime.");
            }

            if (breakItem.StartTime < availabilityStart || breakItem.EndTime > availabilityEnd)
            {
                throw new BadRequestException("Break must be completely inside the availability working hours.");
            }
        }

        var orderedBreaks = breakList.OrderBy(x => x.StartTime).ToList();

        for (var i = 1; i < orderedBreaks.Count; i++)
        {
            if (orderedBreaks[i].StartTime < orderedBreaks[i - 1].EndTime)
            {
                throw new BadRequestException("Availability breaks cannot overlap.");
            }
        }
    }
}