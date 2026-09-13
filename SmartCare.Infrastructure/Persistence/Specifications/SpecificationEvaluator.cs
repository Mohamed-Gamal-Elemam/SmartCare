using Microsoft.EntityFrameworkCore;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Specifications;

namespace SmartCare.Infrastructure.Persistence.Specifications;

public static class SpecificationEvaluator<TEntity> where TEntity : class
{
    public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification)
    {
        var query = inputQuery;

        if (specification.IsNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        // تطبيق الـ Includes العادية
        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        // تطبيق الـ IncludeStrings (مثل ThenInclude)
        query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip!.Value).Take(specification.Take!.Value);
        }

        return query;
    }

    // كويري الـ Count فقط (بدون Includes ولا OrderBy لسرعة الـ Query في Database)
    public static IQueryable<TEntity> GetCountQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> specification)
    {
        var query = inputQuery;

        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        return query;
    }
}