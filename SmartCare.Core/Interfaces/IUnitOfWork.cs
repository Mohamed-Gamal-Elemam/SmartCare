using Microsoft.EntityFrameworkCore.Storage;
using SmartCare.Core.Interfaces.Repositories;
using System.Data;

namespace SmartCare.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : class;
        Task<int> SaveChangesAsync();

        // Transactions Management
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task CommitAsync();
        Task RollbackAsync();
    }
}
