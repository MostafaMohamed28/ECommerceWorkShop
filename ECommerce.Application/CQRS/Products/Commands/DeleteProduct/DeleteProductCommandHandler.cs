using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Commands.DeleteProduct
{
    public class DeleteProductCommandHandler:IRequestHandler<DeleteProductCommand,Result>
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IUnitOfWork unitOfWork)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteProductCommand command, CancellationToken ct = default)
        {
            var product = await _productReadRepository.GetProductForUpdateOrDeleteAsync(command.Id, ct);
            if (product == null)
                return Result.Fail(Error.NotFound("Product.NotFound", $"Product with ID {command.Id} not found."));
            await _productWriteRepository.DeleteProductAsync(product.Id, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
