using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Domain.Contract.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<IDbContextTransaction> GetTransaction(CancellationToken ct = default);
        Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken ct = default) where TEntity : class;
    }
}
