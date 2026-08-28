using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;

namespace ECommerce.Domain.Contract.Orders
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct);
        Task<IReadOnlyList<Order>> GetByCustomerIdAsync(int customerId, CancellationToken ct);
        Task<Coupon?> GetCouponByCodeAsync(string code, CancellationToken ct);
    }
}
