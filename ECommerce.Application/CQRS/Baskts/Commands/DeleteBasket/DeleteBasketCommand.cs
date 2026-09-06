using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.DeleteBasket
{
    public record DeleteBasketCommand(int basketId):IRequest<Result>;
}
