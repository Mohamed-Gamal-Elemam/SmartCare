using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using SmartCare.Shared.Responses;

namespace SmartCare.Services.Abstraction.Services;

public interface IDoctorAvailabilityService
{
    Task<ApiResponse<IReadOnlyList<DoctorAvailabilityDto>>> GetDoctorAvailabilitiesAsync(Guid doctorId);

    Task<ApiResponse<IReadOnlyList<DoctorAvailabilityExceptionDto>>> GetDoctorExceptionsAsync(Guid doctorId);

    Task<ApiResponse<DoctorAvailabilityDto>> CreateAvailabilityAsync(Guid doctorId, CreateDoctorAvailabilityDto dto);

    Task<ApiResponse<DoctorAvailabilityDto>> UpdateAvailabilityAsync(Guid doctorId, Guid availabilityId, UpdateDoctorAvailabilityDto dto);

    Task<ApiResponse<DoctorAvailabilityExceptionDto>> AddExceptionAsync(Guid doctorId, CreateDoctorAvailabilityExceptionDto dto);

    Task<ApiResponse<bool>> DeleteAvailabilityAsync(Guid availabilityId, Guid doctorId);

    Task<ApiResponse<bool>> DeleteExceptionAsync(Guid exceptionId, Guid doctorId);
}