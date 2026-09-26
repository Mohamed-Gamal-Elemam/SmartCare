using SmartCare.Core.Entities.Appointments;


namespace SmartCare.Core.Specifications.Appointments
{
    public class AppointmentFilterPaginatedSpec : BaseSpecification<Appointment>
    {
        public AppointmentFilterPaginatedSpec(AppointmentSpecParams specParams)
            : base(a =>
                (!specParams.PatientId.HasValue || a.PatientId == specParams.PatientId.Value) &&
                (!specParams.DoctorId.HasValue || a.DoctorId == specParams.DoctorId.Value) &&
                (!specParams.Date.HasValue || a.AppointmentDate == specParams.Date.Value) &&
                (!specParams.Status.HasValue || a.Status == specParams.Status.Value))
        {
            AddInclude(a => a.Doctor);
            AddInclude(a => a.Doctor.User);
            AddInclude(a => a.Doctor.Specialty);
            AddInclude(a => a.Patient);
            AddInclude(a => a.Patient.User);

            // الترتيب الأحدث أولاً
            AddOrderByDescending(a => a.AppointmentDate);
            AddOrderByDescending(a => a.StartTime);

            // الترقيم (Pagination)
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);
        }
    }
}
