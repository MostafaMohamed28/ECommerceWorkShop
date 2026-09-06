using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Domain.Contract.Baskets;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.DeleteBasket
{
    public class DeleteBasketCommandHandler: IRequestHandler<DeleteBasketCommand, Result>
    {
        private readonly IBasketWriteRepository _basketWriteRepository;
        private readonly IBasketReadRepository _basketReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteBasketCommandHandler(IBasketWriteRepository basketWriteRepository, IBasketReadRepository basketReadRepository, IUnitOfWork unitOfWork)
        {
            _basketWriteRepository = basketWriteRepository;
            _basketReadRepository = basketReadRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(DeleteBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await _basketReadRepository.GetBasketByIdAsync(request.basketId, cancellationToken);
            if(basket is null)
                return Result.Fail(Error.NotFound("BasketNotFound", $"Basket with id {request.basketId} not found."));

            await _basketWriteRepository.DeleteBasketAsync(basket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
