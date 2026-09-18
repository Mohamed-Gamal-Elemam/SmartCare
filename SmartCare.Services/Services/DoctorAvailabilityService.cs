using AutoMapper;
using SmartCare.Core.Entities.DoctorAvailabilitys;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Specifications.DoctorAvailabilitySpecifications;
using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using SmartCare.Services.Abstraction.Services;

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

    public async Task<IReadOnlyList<DoctorAvailabilityDto>> GetDoctorAvailabilitiesAsync(Guid doctorId)
    {
        var spec = new DoctorAvailabilityWithBreaksSpec(doctorId);
        var availabilities = await _unitOfWork.GetRepository<DoctorAvailability, Guid>().GetBySpecificationAsync(spec);
        return _mapper.Map<IReadOnlyList<DoctorAvailabilityDto>>(availabilities);
    }

    public async Task<IReadOnlyList<DoctorAvailabilityExceptionDto>> GetDoctorExceptionsAsync(Guid doctorId)
    {
        var exceptions = await _unitOfWork.GetRepository<DoctorAvailabilityException, Guid>()
            .FindAsync(x => x.DoctorId == doctorId);

        return _mapper.Map<IReadOnlyList<DoctorAvailabilityExceptionDto>>(exceptions);
    }

    public async Task<DoctorAvailabilityDto> SetAvailabilityAsync(Guid doctorId, CreateDoctorAvailabilityDto dto)
    {
        // Validation: StartTime must be earlier than EndTime
        if (dto.StartTime >= dto.EndTime)
            throw new ArgumentException("StartTime must be earlier than EndTime.");

        // Validation for Breaks
        foreach (var b in dto.Breaks)
        {
            if (b.StartTime >= b.EndTime || b.StartTime < dto.StartTime || b.EndTime > dto.EndTime)
                throw new ArgumentException("Invalid Break time range.");
        }

        var spec = new DoctorAvailabilityWithBreaksSpec(doctorId, dto.DayOfWeek);
        var existingList = await _unitOfWork.GetRepository<DoctorAvailability, Guid>().GetBySpecificationAsync(spec);
        var existing = existingList.FirstOrDefault();

        var availabilityRepo = _unitOfWork.GetRepository<DoctorAvailability, Guid>();
        var breakRepo = _unitOfWork.GetRepository<DoctorAvailabilityBreak, Guid>();

        if (existing != null)
        {
            existing.StartTime = dto.StartTime;
            existing.EndTime = dto.EndTime;
            existing.SlotDurationInMinutes = dto.SlotDurationInMinutes;
            existing.IsActive = true;

            // Clear old breaks
            var oldBreaks = await breakRepo.FindAsync(b => b.DoctorAvailabilityId == existing.Id);
            foreach (var oldBreak in oldBreaks)
            {
                breakRepo.Delete(oldBreak);
            }

            // Assign new breaks
            existing.Breaks = _mapper.Map<List<DoctorAvailabilityBreak>>(dto.Breaks);
            availabilityRepo.Update(existing);
        }
        else
        {
            existing = _mapper.Map<DoctorAvailability>(dto);
            existing.DoctorId = doctorId;
            existing.IsActive = true;

            await availabilityRepo.AddAsync(existing);
        }

        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<DoctorAvailabilityDto>(existing);
    }

    public async Task<DoctorAvailabilityExceptionDto> AddExceptionAsync(Guid doctorId, CreateDoctorAvailabilityExceptionDto dto)
    {
        var exceptionEntity = _mapper.Map<DoctorAvailabilityException>(dto);
        exceptionEntity.DoctorId = doctorId;

        await _unitOfWork.GetRepository<DoctorAvailabilityException, Guid>().AddAsync(exceptionEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DoctorAvailabilityExceptionDto>(exceptionEntity);
    }

    public async Task<bool> DeleteAvailabilityAsync(Guid availabilityId, Guid doctorId)
    {
        var repo = _unitOfWork.GetRepository<DoctorAvailability, Guid>();
        var availability = await repo.GetByIdAsync(availabilityId);

        if (availability == null || availability.DoctorId != doctorId)
            return false;

        repo.Delete(availability);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExceptionAsync(Guid exceptionId, Guid doctorId)
    {
        var repo = _unitOfWork.GetRepository<DoctorAvailabilityException, Guid>();
        var exception = await repo.GetByIdAsync(exceptionId);

        if (exception == null || exception.DoctorId != doctorId)
            return false;

        repo.Delete(exception);
        return await _unitOfWork.SaveChangesAsync() > 0;
    }
}