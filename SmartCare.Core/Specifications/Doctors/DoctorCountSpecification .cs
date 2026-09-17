
using SmartCare.Core.Entities.Doctors;

namespace SmartCare.Core.Specifications.Doctors
{
    public class DoctorCountSpecification : BaseSpecification<Doctor>
    {
        public DoctorCountSpecification(
            Guid? specialtyId,
            Guid? clinicId,
            string? search,
            decimal? minFee,
            decimal? maxFee,
            bool? isActive)
            : base(d =>
                (!specialtyId.HasValue || d.SpecialtyId == specialtyId) &&
                (!clinicId.HasValue || d.ClinicId == clinicId) &&
                (!isActive.HasValue || d.IsActive == isActive) &&
                (!minFee.HasValue || d.ConsultationFee >= minFee) &&
                (!maxFee.HasValue || d.ConsultationFee <= maxFee) &&
                (string.IsNullOrEmpty(search) ||
                 d.User.FirstName.ToLower().Contains(search.ToLower()) ||
                 d.User.LastName.ToLower().Contains(search.ToLower()) ||
                 d.Specialty.Name.ToLower().Contains(search.ToLower())))
        {
        }
    }
}
