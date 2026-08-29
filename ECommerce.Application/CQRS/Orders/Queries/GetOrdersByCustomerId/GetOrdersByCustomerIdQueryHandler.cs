using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Domain.Contract.Orders;
using MediatR;

namespace ECommerce.Application.CQRS.Orders.Queries.GetOrdersByCustomerId
{
    public class GetOrdersByCustomerIdQueryHandler : IRequestHandler<GetOrdersByCustomerIdQuery, Result<IReadOnlyList<Order>>>
    {
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly ICustomerReadRepository _customerReadRepository;
        public GetOrdersByCustomerIdQueryHandler(IOrderReadRepository orderReadRepository, ICustomerReadRepository customerReadRepository)
        {
            _orderReadRepository = orderReadRepository;
            _customerReadRepository = customerReadRepository;
        }
        public async Task<Result<IReadOnlyList<Order>>> Handle(GetOrdersByCustomerIdQuery request, CancellationToken ct = default)
        {
            var customerExists = await _customerReadRepository.ExistsAsync(request.CustomerId, ct);
            if (!customerExists)
                return Result<IReadOnlyList<Order>>.Fail(Error.NotFound("Customer.NotFound", $"Customer with ID {request.CustomerId} not found."));
            var orders = await _orderReadRepository.GetByCustomerIdAsync(request.CustomerId, ct);
            return Result<IReadOnlyList<Order>>.Ok(orders);
        }
    }
}
