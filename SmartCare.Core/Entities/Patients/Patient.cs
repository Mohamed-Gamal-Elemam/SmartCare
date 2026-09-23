using SmartCare.Core.Entities.Appointments;
using SmartCare.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Entities.Patients
{
    public class Patient : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? BloodGroup { get; set; }
        public string? Address { get; set; }
        public string? EmergencyContact { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
