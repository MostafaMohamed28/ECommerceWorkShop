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

namespace ECommerce.Application.CQRS.Baskts.Commands.AddItemToBasket
{
    public class AddItemToBasketCommandHandler : IRequestHandler<AddItemToBasketCommand, Result<BasketDto>>
    {
        private readonly IBasketReadRepository _readRepository;
        private readonly IBasketWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddItemToBasketCommandHandler(
            IBasketReadRepository readRepository,
            IBasketWriteRepository writeRepository,
            IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<BasketDto>> Handle(AddItemToBasketCommand request, CancellationToken ct)
        {
            var basket = await _readRepository.GetBasketByCustomerIdAsync(request.CustomerId, ct);

            // Get-or-create: لو مفيش Basket، اعمل واحد جديد
            basket ??= await _writeRepository.AddBasketAsync(request.CustomerId, ct);

            basket.AddItem(request.ProductId, request.Quantity);
            await _writeRepository.UpdateBasketAsync(basket, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var basketDto = new BasketDto
            {
                Id = basket.Id,
                CustomerId = basket.CustomerId,
                CouponCode = basket.CouponCode,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };


            return Result<BasketDto>.Ok(basketDto);
        }
    }
}
