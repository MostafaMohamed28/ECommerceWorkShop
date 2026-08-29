using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;

namespace ECommerce.Domain.Contract.Orders
{
    public interface IOrderReadRepository
    {
        Task<Order?> GetOrderByIdAsync(int id, CancellationToken ct);//Read
        Task<IReadOnlyList<Order>> GetByCustomerIdAsync(int customerId, CancellationToken ct);//Read
        Task<Coupon?> GetCouponByCodeAsync(string code, CancellationToken ct);//Read
    }
}
