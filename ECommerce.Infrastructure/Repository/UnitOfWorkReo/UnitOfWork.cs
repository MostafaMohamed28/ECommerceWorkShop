using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contract.UnitOfWork;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Infrastructure.Repository.UnitOfWorkReo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken ct = default) where TEntity : class
        {
            var result = await _dbContext.Set<TEntity>().AddAsync(entity, ct);
            return result.Entity;
        }

        public async Task<IDbContextTransaction> GetTransaction(CancellationToken ct = default)
        {
             var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
             return transaction;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _dbContext.SaveChangesAsync(ct);
        }
    }
}
