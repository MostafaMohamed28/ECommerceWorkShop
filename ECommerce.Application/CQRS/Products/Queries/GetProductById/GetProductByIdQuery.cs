using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Queries.GetProductByIds
{
    public record GetProductByIdQuery(int Id):IRequest<Result<Product>>;
}
