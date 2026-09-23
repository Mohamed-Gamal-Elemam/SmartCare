using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.DTOs.DoctorAvailability
{
    public class TimeSlotDto
    {

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
        public string? UnavailabilityReason { get; set; }
        public bool IsAvailable { get; set; }

    }
}
