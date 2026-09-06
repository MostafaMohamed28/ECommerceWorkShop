using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Baskets;
using ECommerce.Domain.Entities.Basket;
using ECommerce.Domain.Entities.Baskets;
using ECommerce.Infrastructure.Context;
using ECommerce.Infrastructure.Repository.UnitOfWorkReo;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repository.BasketRepo
{
    public class BasketReadRepository : IBasketReadRepository
    {
        private readonly AppDbContext _context;
        public BasketReadRepository(AppDbContext context)
        {
            _context = context; 
        }

        public async Task<Basket?> GetBasketByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            var result=await _context.Baskets.Include(b=>b.Items).ThenInclude(p=>p.Product).FirstOrDefaultAsync(b => b.CustomerId == customerId, ct);
            return result;
        }

        public async Task<Basket?> GetBasketByIdAsync(int basketId, CancellationToken ct=default)
        {
           var basket =await _context.Baskets.Include(i=>i.Items).ThenInclude(p=>p.Product).FirstOrDefaultAsync(b => b.Id == basketId, ct);
           return basket;
        }
    }

    public class BasketWriteRepository : IBasketWriteRepository
    {
        private readonly AppDbContext _context;
        public BasketWriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Basket> AddBasketAsync(int CustomerId, CancellationToken ct=default)
        {
            var basket = new Basket(CustomerId); // constructor بدل object initializer
            await _context.Baskets.AddAsync(basket, ct);
            await _context.SaveChangesAsync(ct);
            return basket;
        }

        public async Task DeleteBasketAsync(Basket basket, CancellationToken ct=default)
        {
          
            _context.Baskets.Remove(basket);
        }

        public async Task<int> RemoveStaleItemsAsync(DateTime cutoffDate, CancellationToken ct = default)
        {
           return await _context.BasketItems.Where(i => i.AddedAt < cutoffDate).ExecuteDeleteAsync(ct);
        }

        public async Task UpdateBasketAsync(Basket basket, CancellationToken ct=default)
        {
           var Basket =_context.Baskets.Update(basket);


                
        }
    }
}
