using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Orders;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repository.OrdersRepo
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(int customerId, CancellationToken ct)
        {
            var orders =await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .AsNoTracking()
                .ToListAsync(ct);

            return orders;
        }

        public async Task<Coupon?> GetCouponByCodeAsync(string code, CancellationToken ct)
        {
            var coupon = await _context.Coupons
                  .FirstOrDefaultAsync(c => c.Code.ToUpper() == code.ToUpper() && c.IsActive, ct);
            return coupon;
        }

        public async Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct)
        {

            return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        }
    }
}
