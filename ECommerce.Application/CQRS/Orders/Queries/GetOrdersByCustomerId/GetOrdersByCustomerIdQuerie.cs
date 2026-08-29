using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using MediatR;

namespace ECommerce.Application.CQRS.Orders.Queries.GetOrdersByCustomerId
{
    public record GetOrdersByCustomerIdQuery(int CustomerId) : IRequest<Result<IReadOnlyList<Order>>>;
}
