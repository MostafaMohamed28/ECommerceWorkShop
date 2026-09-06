using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.BasktDtos;
using ECommerce.Domain.Contract.Baskets;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Queries.GetBasketByCustomerId
{
    public class GetBasketByCustomerIdQueryHandler: IRequestHandler<GetBasketByCustomerIdQuery, Result<BasketDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketReadRepository _basketReadRepository;
        public GetBasketByCustomerIdQueryHandler(IUnitOfWork unitOfWork, IBasketReadRepository basketReadRepository)
        {
            _unitOfWork = unitOfWork;
            _basketReadRepository = basketReadRepository;
        }


        public async Task<Result<BasketDto?>> Handle(GetBasketByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketReadRepository.GetBasketByCustomerIdAsync(request.CustomerId, cancellationToken);
            if (basket == null)
                return Result<BasketDto?>.Fail(new Error("BasketNotFound", $"Basket not found for customer with ID {request.CustomerId}."));


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
            return Result<BasketDto?>.Ok(basketDto);
        }

    }
}
