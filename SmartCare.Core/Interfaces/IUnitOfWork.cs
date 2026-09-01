using SmartCare.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartCare.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity, TId> GetRepository<TEntity, TId>() where TEntity : class;
        Task<int> SaveChangesAsync();
    }
}
