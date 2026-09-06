using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Contracts.Redis;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Queries.GetAllProduct
{
    public class GetAllProductsQueryHandler:IRequestHandler<GetAllProductsQuery, Result<IReadOnlyList<Product>>>
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly ICacheService<Product> _cacheService;

        public GetAllProductsQueryHandler(IProductReadRepository productReadRepository, ICacheService<Product> cacheService)
        {
            _productReadRepository = productReadRepository;
            _cacheService = cacheService;
        }

        public async Task<Result<IReadOnlyList<Product>>> Handle(GetAllProductsQuery query, CancellationToken ct = default)
        {
            var products = await _productReadRepository.GetAllProductsAsync(ct);

            return Result<IReadOnlyList<Product>>.Ok(products);
        }

    }
}
