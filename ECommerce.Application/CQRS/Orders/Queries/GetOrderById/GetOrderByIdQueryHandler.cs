using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Orders;
using MediatR;

namespace ECommerce.Application.CQRS.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler:IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
    {
        private readonly IOrderReadRepository _orderReadRepository;
        public GetOrderByIdQueryHandler(IOrderReadRepository orderReadRepository)
        {
            _orderReadRepository = orderReadRepository;
        }
        public async Task<Result<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
        {
            var order = await _orderReadRepository.GetOrderByIdAsync(request.Id, ct);
            if (order == null)
            {
                return Result<OrderDto>.Fail(Error.NotFound("OrderNotFound", $"Order with ID {request.Id} not found."));
            }

            var orderDto = new OrderDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DiscountAmount = order.DiscountAmount,
                TaxAmount = order.TaxAmount,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(item => new OrderItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList(),
                Payment = order.Payment
            };
            return Result<OrderDto>.Ok(orderDto);
        }
    }
}
