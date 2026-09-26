using SmartCare.Services.Abstraction.DTOs.DoctorAvailability;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.Services
{
    public interface ITimeSlotService
    {
        Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(Guid doctorId,DateOnly date);
    }
}
