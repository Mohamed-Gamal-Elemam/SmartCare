using SmartCare.Core.Entities.Climic;
using SmartCare.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Entities.Docotr
{
    public class Doctor : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }

        public Guid SpecialtyId { get; set; }

        public Guid ClinicId { get; set; }

        public string Biography { get; set; } = null!;

        public int YearsOfExperience { get; set; }

        public decimal ConsultationFee { get; set; }

        public bool IsActive { get; set; } = true;


        // Navigation Properties

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual Specialty Specialty { get; set; } = null!;

        public virtual Clinic Clinic { get; set; } = null!;
    }
}
