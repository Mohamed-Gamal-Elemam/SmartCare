using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Services.Abstraction.DTOs.DoctorAvailability
{
    public class CreateDoctorAvailabilityExceptionDto
    {
        public DateOnly Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsUnavailable { get; set; }
        public string? Reason { get; set; }
    }
}
