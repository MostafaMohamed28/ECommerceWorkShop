using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.BasktDtos;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.AddItemToBasket
{
    public record AddItemToBasketCommand(int CustomerId, int ProductId, int Quantity):IRequest<Result<BasketDto>>;
}
