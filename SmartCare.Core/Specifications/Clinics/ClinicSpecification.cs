
using System.Linq.Expressions;
using SmartCare.Core.Entities;

namespace SmartCare.Core.Specifications.Clinics;

public class ClinicSpecification : BaseSpecification<Clinic>
{
    public ClinicSpecification( string? search, bool? isActive, int pageNumber, int pageSize)
    {
        Criteria = BuildCriteria(search, isActive);

        AddInclude(c => c.WorkingHours);
        AddOrderBy(c => c.Name);
        ApplyPaging(pageNumber, pageSize);
    }

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