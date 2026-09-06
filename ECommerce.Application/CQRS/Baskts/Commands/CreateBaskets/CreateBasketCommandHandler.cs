using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.BasktDtos;
using ECommerce.Domain.Contract;
using ECommerce.Domain.Contract.Baskets;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.CreateBaskets
{
    public class CreateBasketCommandHandler: IRequestHandler<CreateBasketCommand, Result<BasketDto>>
    {
        private readonly IBasketWriteRepository _basketWriteRepository;
        private readonly IBasketReadRepository _basketReadRepository;
        private readonly IUnitOfWork _unitofwork;
        private readonly ICustomerReadRepository _customerReadRepository;
        public CreateBasketCommandHandler(IBasketWriteRepository basketWriteRepository, IBasketReadRepository basketReadRepository, IUnitOfWork unitofwork, ICustomerReadRepository customerReadRepository)
        {
            _basketWriteRepository = basketWriteRepository;
            _basketReadRepository = basketReadRepository;
            _unitofwork = unitofwork;
            _customerReadRepository = customerReadRepository;
        }

        public async Task<Result<BasketDto>> Handle(CreateBasketCommand command, CancellationToken ct = default)
        {
            var customer= await _customerReadRepository.GetByIdAsync(command.customerId, ct);

            if (customer is null)
                return Result<BasketDto>.Fail(Error.NotFound("CustomerNotFound", $"Customer with ID {command.customerId} not found."));


            var basket = await _basketWriteRepository.AddBasketAsync(command.customerId, ct);
            await _unitofwork.SaveChangesAsync(ct);
            var basketDto = new BasketDto
            {
                Id = basket.Id,
                CustomerId = basket.CustomerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    Quantity = item.Quantity
                }).ToList()
            };

            return Result<BasketDto>.Ok(basketDto);
        }

    }
}
