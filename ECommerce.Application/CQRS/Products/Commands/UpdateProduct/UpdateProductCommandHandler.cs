using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler:IRequestHandler<UpdateProductCommand, Result<Product>>
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository, IUnitOfWork unitOfWork)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Product>> Handle(UpdateProductCommand command, CancellationToken ct = default)
        {
            var productexisting = await _productReadRepository.GetProductForUpdateOrDeleteAsync(command.Id, ct);
            if (productexisting == null)
                return Result<Product>.Fail(Error.NotFound("Product.NotFound", $"Product with ID {command.Id} not found."));
            if (command.Price <= 0)
                return Result<Product>.Fail(Error.Validation("Price must be positive."));

            productexisting.Name = command.Name;
            productexisting.SKU = command.SKU;
            productexisting.Price = command.Price;
            productexisting.StockQuantity = command.StockQuantity;

            await _productWriteRepository.UpdateProductAsync(productexisting, ct);

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Product>.Ok(productexisting);
        }
    }
}
