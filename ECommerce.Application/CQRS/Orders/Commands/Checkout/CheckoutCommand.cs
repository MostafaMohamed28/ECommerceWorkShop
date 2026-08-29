using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using MediatR;

namespace ECommerce.Application.CQRS.Orders.Commands.Checkout
{
    public record CheckoutCommand(CreateOrderDto request) :IRequest<Result<CheckoutResponseDto>>;
}
