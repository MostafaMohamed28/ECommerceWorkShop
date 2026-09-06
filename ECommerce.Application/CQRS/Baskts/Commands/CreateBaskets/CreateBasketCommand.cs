using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.BasktDtos;
using ECommerce.Domain.Entities.Basket;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.CreateBaskets
{
    public record CreateBasketCommand(int customerId) : IRequest<Result<BasketDto>>;
}
