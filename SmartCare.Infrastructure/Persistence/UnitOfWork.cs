using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartCare.Core.Interfaces;
using SmartCare.Core.Interfaces.Repositories;
using SmartCare.Infrastructure.Persistence.Repositories;
using System.Data;

namespace SmartCare.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly SmartCareDbContext _context;
    private readonly Dictionary<string, object> _repositories = new();
    private IDbContextTransaction? _currentTransaction;

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

    public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (_currentTransaction != null)
        {
            return _currentTransaction;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(isolationLevel);
        return _currentTransaction;
    }

    public async Task CommitAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}