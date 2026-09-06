using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.RemoveItems
{
    public record RemoveItemFromBasketCommand(int CustomerId, int ProductId): IRequest<Result<Unit>>;
}
