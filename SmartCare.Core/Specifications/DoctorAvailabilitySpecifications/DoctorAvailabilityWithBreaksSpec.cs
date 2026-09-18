using SmartCare.Core.Entities.DoctorAvailabilitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Specifications.DoctorAvailabilitySpecifications
{
    public class DoctorAvailabilityWithBreaksSpec : BaseSpecification<DoctorAvailability>
    {
        public DoctorAvailabilityWithBreaksSpec(Guid doctorId, DayOfWeek dayOfWeek)
            : base(x => x.DoctorId == doctorId && x.DayOfWeek == dayOfWeek && x.IsActive)
        {
            AddInclude(x => x.Breaks);
        }

        public DoctorAvailabilityWithBreaksSpec(Guid doctorId)
            : base(x => x.DoctorId == doctorId && x.IsActive)
        {
            AddInclude(x => x.Breaks);
        }
    }
}
