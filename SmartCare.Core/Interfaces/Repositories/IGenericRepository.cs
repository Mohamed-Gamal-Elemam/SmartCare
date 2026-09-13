using System.Linq.Expressions;

namespace SmartCare.Core.Interfaces.Repositories
{
    public interface IGenericRepository<TEntity, TId> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TId id);
        Task<IReadOnlyList<TEntity>> GetAllTrackedAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IReadOnlyList<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> specification);

        Task<int> CountAsync( ISpecification<TEntity> specification);

        Task AddAsync(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);
        
    }

}
