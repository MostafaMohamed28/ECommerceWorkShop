using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using MediatR;

namespace ECommerce.Application.CQRS.Orders.Queries.GetOrderById
{
    public record GetOrderByIdQuery(int Id):IRequest<Result<OrderDto>>;
}
