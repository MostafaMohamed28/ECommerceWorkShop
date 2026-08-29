using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using MediatR;

namespace ECommerce.Application.CQRS.Customers.Commands.UpgradeToVip
{
    public record UpgradeToVipCommand(int customerId): IRequest<Result>
    {
    }
}
