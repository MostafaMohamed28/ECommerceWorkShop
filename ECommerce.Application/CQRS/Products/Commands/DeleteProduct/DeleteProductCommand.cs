using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand(int Id):IRequest<Result>;
}
