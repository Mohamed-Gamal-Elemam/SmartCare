using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.DTOs.DoctorAvailability
{
    public class CreateDoctorAvailabilityDto
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotDurationInMinutes { get; set; } = 30;
        public List<CreateDoctorAvailabilityBreakDto> Breaks { get; set; } = new();
    }
}
