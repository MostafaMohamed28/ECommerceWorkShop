using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;

namespace ECommerce.Application.Contracts.OrderService
{
    public interface IOrderService
    {
        Task<Result<Order>> GetOrderByIdAsync(int id, CancellationToken ct = default);

        Task<Result<IReadOnlyList<Order>>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken ct = default);
        Task<Result> CancelOrderAsync(int id,CancellationToken ct = default);
        Task<Result<CheckoutResponseDto>> CheckoutAsync(CreateOrderDto request, CancellationToken ct = default);

    }
}
