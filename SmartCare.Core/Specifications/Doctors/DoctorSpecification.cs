

using SmartCare.Core.Entities.Doctors;

namespace SmartCare.Core.Specifications.Doctors
{
    public class DoctorSpecification : BaseSpecification<Doctor>
    {
        // Constructor for Paginated & Filtered Query
        public DoctorSpecification(
            Guid? specialtyId,
            Guid? clinicId,
            string? search,
            decimal? minFee,
            decimal? maxFee,
            bool? isActive,
            string? sort,
            int skip,
            int take)
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
            AddInclude(d => d.User);
            AddInclude(d => d.Specialty);
            AddInclude(d => d.Clinic);

            ApplyPaging(skip, take);

            if (!string.IsNullOrEmpty(sort))
            {
                switch (sort.ToLower())
                {
                    case "feeasc":
                        AddOrderBy(d => d.ConsultationFee);
                        break;
                    case "feedesc":
                        AddOrderByDescending(d => d.ConsultationFee);
                        break;
                    case "experience":
                        AddOrderByDescending(d => d.YearsOfExperience);
                        break;
                    default:
                        AddOrderByDescending(d => d.User.CreatedAt);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(d => d.User.CreatedAt);
            }
        }

        // Constructor for Single Entity Fetch by Id
        public DoctorSpecification(Guid id)
            : base(d => d.Id == id)
        {
            AddInclude(d => d.User);
            AddInclude(d => d.Specialty);
            AddInclude(d => d.Clinic);
        }


    }
}