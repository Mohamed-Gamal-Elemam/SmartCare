using SmartCare.Core.Entities.Appointments;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Specifications.Appointments
{
    public class AppointmentOverlappingSpec : BaseSpecification<Appointment>
    {
        public AppointmentOverlappingSpec(Guid doctorId, DateOnly appointmentDate, TimeOnly startTime, TimeOnly endTime)
            : base(a => a.DoctorId == doctorId
                     && a.AppointmentDate == appointmentDate
                     && a.Status != AppointmentStatus.Cancelled
                     // Overlap logic: Start1 < End2 AND Start2 < End1
                     && a.StartTime < endTime
                     && startTime < a.EndTime)
        {
        }
    }
}
