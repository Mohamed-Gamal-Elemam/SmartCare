using SmartCare.Core.Entities.DoctorAvailabilitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Specifications.DoctorAvailabilitySpecifications
{
    public class DoctorAvailabilityExceptionSpec : BaseSpecification<DoctorAvailabilityException>
    {
        public DoctorAvailabilityExceptionSpec(Guid doctorId, DateOnly date)
            : base(x => x.DoctorId == doctorId && x.Date == date)
        {
        }
    }
}
