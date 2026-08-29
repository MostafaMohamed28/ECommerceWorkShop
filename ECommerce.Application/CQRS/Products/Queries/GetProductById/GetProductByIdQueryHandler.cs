using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.CQRS.Products.Queries.GetProductByIds;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Queries.GetProductById
{
    public class GetProductByIdQueryHandler: IRequestHandler<GetProductByIdQuery, Result<Product>>
    {
        private readonly IProductReadRepository _productReadRepository;

        public GetProductByIdQueryHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }
        public async Task<Result<Product>> Handle(GetProductByIdQuery query, CancellationToken ct = default)
        {
            var product = await _productReadRepository.GetProductByIdAsync(query.Id, ct);
            if (product == null)
            {
                return Result<Product>.Fail(Error.NotFound("Product.NotFound", $"Product with ID {query.Id} not found."));
            }
            return Result<Product>.Ok(product);
        }

    }
}
