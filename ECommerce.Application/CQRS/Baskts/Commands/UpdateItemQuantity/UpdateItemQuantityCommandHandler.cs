using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.BasktDtos;
using ECommerce.Domain.Contract.Baskets;
using ECommerce.Domain.Contract.UnitOfWork;
using ECommerce.Domain.Entities.Baskets;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.UpdateBasket
{
    public class UpdateItemQuantityCommandHandler
         : IRequestHandler<UpdateItemQuantityCommand, Result<BasketDto>>
    {
        private readonly IBasketReadRepository _basketReadRepository;
        private readonly IBasketWriteRepository _basketWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateItemQuantityCommandHandler(
            IBasketReadRepository basketReadRepository,
            IBasketWriteRepository basketWriteRepository,
            IUnitOfWork unitOfWork)
        {
            _basketReadRepository = basketReadRepository;
            _basketWriteRepository = basketWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<BasketDto>> Handle(UpdateItemQuantityCommand request, CancellationToken ct)
        {
            var basket = await _basketReadRepository.GetBasketByIdAsync(request.basketId, ct);
            if (basket is null)
                return Result<BasketDto>.Fail(Error.NotFound("Basket not found", "Basket not found for the given basket id."));

            if (!basket.Items.Any(i => i.ProductId == request.productId))
                return Result<BasketDto>.Fail(Error.NotFound("Item not found", "This product does not exist in the basket."));

            try
            {
                basket.UpdateItemQuantity(request.productId, request.newQuantity);
            }
            catch (ArgumentException ex)
            {
                return Result<BasketDto>.Fail(Error.Validation("Invalid quantity", ex.Message));
            }

            await _basketWriteRepository.UpdateBasketAsync(basket, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var basketDto = new BasketDto
            {
                Id =basket.Id,
                CustomerId = basket.CustomerId,
                CouponCode = basket.CouponCode,
                Items = basket.Items.Select(i => new BasketItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                }).ToList()
            };

            return Result<BasketDto>.Ok(basketDto);
        }
    }
}
