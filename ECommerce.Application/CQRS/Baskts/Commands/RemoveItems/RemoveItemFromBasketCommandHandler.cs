using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Domain.Contract.Baskets;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Commands.RemoveItems
{
    public class RemoveItemFromBasketCommandHandler: IRequestHandler<RemoveItemFromBasketCommand, Result<Unit>>
    {
        private readonly IBasketReadRepository _readRepository;
        private readonly IBasketWriteRepository _writeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RemoveItemFromBasketCommandHandler(IBasketReadRepository readRepository, IBasketWriteRepository writeRepository, IUnitOfWork unitOfWork)
        {
            _readRepository = readRepository;
            _writeRepository = writeRepository; 
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<Unit>> Handle(RemoveItemFromBasketCommand request, CancellationToken ct)
        {
            var basket = await _readRepository.GetBasketByCustomerIdAsync(request.CustomerId, ct);
            if (basket == null)
                return Result<Unit>.Fail(Error.NotFound("Basket not found", "Basket not found for the given customer id."));

            basket.RemoveItem(request.ProductId);
            await _writeRepository.UpdateBasketAsync(basket, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Unit>.Ok(Unit.Value);
        }
    }
}
