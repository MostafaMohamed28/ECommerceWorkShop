using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand(int Id, string Name, string SKU, decimal Price, int StockQuantity):IRequest<Result<Product>>;

}
