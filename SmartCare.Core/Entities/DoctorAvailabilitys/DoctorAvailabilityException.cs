using SmartCare.Core.Entities.Doctors;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Entities.DoctorAvailabilitys
{
    public class DoctorAvailabilityException : BaseEntity<Guid>
    {
        public Guid DoctorId { get; set; }

        public DateOnly Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public bool IsUnavailable { get; set; }

        public string? Reason { get; set; }

        // Navigation Property
        public Doctor Doctor { get; set; } = null!;
    }
}
