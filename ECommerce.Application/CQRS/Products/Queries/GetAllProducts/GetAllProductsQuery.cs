using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Queries.GetAllProduct
{
    public record GetAllProductsQuery:IRequest<Result<IReadOnlyList<Product>>>;

}
