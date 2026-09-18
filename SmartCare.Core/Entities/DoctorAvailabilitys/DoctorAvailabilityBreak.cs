using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Entities.DoctorAvailabilitys
{
    public class DoctorAvailabilityBreak : BaseEntity<Guid>
    {
        public Guid DoctorAvailabilityId { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        // Navigation Property
        public DoctorAvailability DoctorAvailability { get; set; } = null!;
    }
}
