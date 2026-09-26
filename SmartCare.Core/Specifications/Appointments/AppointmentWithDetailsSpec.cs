using SmartCare.Core.Entities.Appointments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Specifications.Appointments
{
    public class AppointmentWithDetailsSpec : BaseSpecification<Appointment>
    {
        // لجلب حجز محدد بالـ Id
        public AppointmentWithDetailsSpec(Guid id)
            : base(a => a.Id == id)
        {
            AddIncludes();
        }

        // لجلب حجز محدد وتأكيد ملكيته لمريض معين (مستوى حماية إضافي)
        public AppointmentWithDetailsSpec(Guid id, Guid patientId)
            : base(a => a.Id == id && a.PatientId == patientId)
        {
            AddIncludes();
        }

        private void AddIncludes()
        {
            AddInclude(a => a.Doctor);
            AddInclude(a => a.Doctor.User);
            AddInclude(a => a.Doctor.Specialty);
            AddInclude(a => a.Doctor.Clinic);
            AddInclude(a => a.Patient);
            AddInclude(a => a.Patient.User);
        }
    }
}
