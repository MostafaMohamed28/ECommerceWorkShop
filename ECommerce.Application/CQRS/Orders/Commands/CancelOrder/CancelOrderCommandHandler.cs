using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Orders;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Orders.Commands.CancelOrder
{
    public class CancelOrderCommandHandler:IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CancelOrderCommandHandler(IOrderReadRepository orderReadRepository, IUnitOfWork unitOfWork)
        {
            _orderReadRepository = orderReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(CancelOrderCommand request, CancellationToken ct = default)
        {
            var order = await _orderReadRepository.GetOrderByIdAsync(request.Id, ct);
            if (order == null)
            {
                return Result.Fail(Error.NotFound("Order.NotFound", $"Order with ID {request.Id} not found."));
            }
            if (order.Status == OrderStatus.Cancelled)
            {
                return Result.Fail(Error.Conflict("Order.AlreadyCancelled", $"Order with ID {request.Id} is already cancelled."));
            }

            if (order.Status == OrderStatus.Paid)
            {
                foreach (var item in order.Items)
                {
                    var product = item.Product;
                    if (product is not null)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                }
            }
            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }
    }
}
