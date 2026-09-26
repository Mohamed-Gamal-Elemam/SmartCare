
using System.Linq.Expressions;
using SmartCare.Core.Entities.Climic;

namespace SmartCare.Core.Specifications.Clinics;

public class ClinicSpecification : BaseSpecification<Clinic>
{
    public ClinicSpecification(ClinicSpecParams specParams)
    {
        Criteria = BuildCriteria(specParams.Search, specParams.IsActive);

        AddInclude(c => c.WorkingHours);
        AddOrderBy(c => c.Name);

        // الترقيم: skip = PageSize * (PageNumber - 1)
        ApplyPaging(specParams.PageSize * (specParams.PageNumber - 1), specParams.PageSize);
    }

    // Constructor لجلب عيادة بالـ Id
    public ClinicSpecification(Guid id) : base(c => c.Id == id)
    {
        AddInclude(c => c.WorkingHours);
    }

    private static Expression<Func<Clinic, bool>>? BuildCriteria(string? search, bool? isActive)
    {
        if (string.IsNullOrWhiteSpace(search) && isActive is null)
            return null;

        var cleanSearch = search?.Trim().ToLower();

        return c =>
            (string.IsNullOrEmpty(cleanSearch) ||
             c.Name.ToLower().Contains(cleanSearch) ||
             c.Address.ToLower().Contains(cleanSearch) ||
             c.PhoneNumber.Contains(cleanSearch))
            &&
            (isActive == null || c.IsActive == isActive);
    }
}