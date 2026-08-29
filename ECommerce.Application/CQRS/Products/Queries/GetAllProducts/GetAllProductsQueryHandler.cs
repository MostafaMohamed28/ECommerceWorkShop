using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Queries.GetAllProduct
{
    public class GetAllProductsQueryHandler:IRequestHandler<GetAllProductsQuery, Result<IReadOnlyList<Product>>>
    {
        private readonly IProductReadRepository _productReadRepository;

        public GetAllProductsQueryHandler(IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<Result<IReadOnlyList<Product>>> Handle(GetAllProductsQuery query, CancellationToken ct = default)
        {
            var products = await _productReadRepository.GetAllProductsAsync(ct);

            return Result<IReadOnlyList<Product>>.Ok(products);
        }

    }
}
