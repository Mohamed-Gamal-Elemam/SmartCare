using SmartCare.Core.Entities.Appointments;

namespace SmartCare.Core.Specifications.Appointments
{
    public class AppointmentByDoctorAndDateSpec : BaseSpecification<Appointment>
    {
        public AppointmentByDoctorAndDateSpec(Guid doctorId, DateOnly date)
            : base(a => a.DoctorId == doctorId
                     && a.AppointmentDate == date
                     && a.Status != AppointmentStatus.Cancelled)
        {
          
            AddOrderBy(a => a.StartTime);
        }
    }
}