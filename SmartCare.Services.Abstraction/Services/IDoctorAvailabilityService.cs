using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.Services
{
    public interface IDoctorAvailabilityService
    {
        Task<IReadOnlyList<DoctorAvailabilityDto>> GetDoctorAvailabilitiesAsync(Guid doctorId);
        Task<IReadOnlyList<DoctorAvailabilityExceptionDto>> GetDoctorExceptionsAsync(Guid doctorId);
        Task<DoctorAvailabilityDto> SetAvailabilityAsync(Guid doctorId, CreateDoctorAvailabilityDto dto);
        Task<DoctorAvailabilityExceptionDto> AddExceptionAsync(Guid doctorId, CreateDoctorAvailabilityExceptionDto dto);
        Task<bool> DeleteAvailabilityAsync(Guid availabilityId, Guid doctorId);
        Task<bool> DeleteExceptionAsync(Guid exceptionId, Guid doctorId);
    }
}
