using SmartCare.Core.Entities.Doctors;


namespace SmartCare.Core.Entities.DoctorAvailabilitys
{
    public class DoctorAvailability : BaseEntity<Guid>
    {
        public Guid DoctorId { get; set; }

        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int SlotDurationInMinutes { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Property
        public Doctor Doctor { get; set; } = null!;
        public ICollection<DoctorAvailabilityBreak> Breaks { get; set; } = new List<DoctorAvailabilityBreak>();
    }
}
