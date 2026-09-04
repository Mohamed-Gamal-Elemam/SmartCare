using Microsoft.EntityFrameworkCore;
using SmartCare.Core.Interfaces.Repositories;
using System.Linq.Expressions;

namespace SmartCare.Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity, TId>
    : IGenericRepository<TEntity, TId>
    where TEntity : class
{
    private readonly SmartCareDbContext _context;

    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(SmartCareDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<IReadOnlyList<TEntity>> GetAllTrackedAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }
}