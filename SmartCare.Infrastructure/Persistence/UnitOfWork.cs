using SmartCare.Core.Interfaces;
using SmartCare.Core.Interfaces.Repositories;
using SmartCare.Infrastructure.Persistence.Repositories;

namespace SmartCare.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SmartCareDbContext _context;

    private readonly Dictionary<string, object> _repositories = new();

    public UnitOfWork(SmartCareDbContext context)
    {
        _context = context;
    }


    public IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : class
    {
        var entityName = typeof(TEntity).FullName!;

        if (!_repositories.TryGetValue(entityName, out var repository))
        {
            repository = new GenericRepository<TEntity, TId>(_context);

            _repositories.Add(entityName, repository);
        }

        return (IGenericRepository<TEntity, TId>)repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}