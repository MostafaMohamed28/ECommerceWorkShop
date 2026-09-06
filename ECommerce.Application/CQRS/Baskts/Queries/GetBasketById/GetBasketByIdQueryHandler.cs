using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.CQRS.Baskts.Queries.GetBasketByIds;
using ECommerce.Application.DTOs.BasktDtos;
using ECommerce.Domain.Contract.Baskets;
using ECommerce.Domain.Entities.Basket;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Queries.GetBasketById
{
    public class GetBasketByIdQueryHandler : IRequestHandler<GetBasketByIdQuery, Result<BasketDto>>
    {
        private readonly IBasketReadRepository _basketReadRepository;
        public GetBasketByIdQueryHandler(IBasketReadRepository basketReadRepository)
        {
            _basketReadRepository = basketReadRepository;
        }

        public async Task<Result<BasketDto>> Handle(GetBasketByIdQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketReadRepository.GetBasketByIdAsync(request.basketId, cancellationToken);
            if (basket == null)
            {
                return Result<BasketDto>.Fail(Error.NotFound("BasketNot.Found",$" Basket {request.basketId} not found"));
            }
            var basketDto = new BasketDto
            {
                Id = basket.Id,
                CustomerId = basket.CustomerId,
                CouponCode = basket.CouponCode,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    Price = item.Product?.Price ?? 0,
                    Quantity = item.Quantity
                }).ToList()
            };
            return Result<BasketDto>.Ok(basketDto);
        }
    }
}
