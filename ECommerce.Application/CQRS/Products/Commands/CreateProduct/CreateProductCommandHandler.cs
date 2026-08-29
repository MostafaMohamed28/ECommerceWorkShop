using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.CQRS.Products.Commands.CreateProduct;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Products.Commands.CreateProducts
{
    public class CreateProductCommandHandler:IRequestHandler<CreateProductCommand, Result<Product>>
    {
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IUnitOfWork _unitOfWork ;



        public CreateProductCommandHandler(IProductWriteRepository productWriteRepository, IProductReadRepository productReadRepository,IUnitOfWork unitOfWork)
        {
            _productWriteRepository = productWriteRepository;
            _productReadRepository = productReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Product>> Handle(CreateProductCommand command, CancellationToken ct = default)
        {

            if (command.Price <= 0)
                return Result<Product>.Fail(Error.Validation("Price must be greater than zero."));


            if (command.StockQuantity < 0)
                return Result<Product>.Fail(Error.Validation("Stock quantity cannot be negative."));


            var skuExists = await _productReadRepository.ProductExistAsync(command.SKU, ct);
            if (skuExists)
                return Result<Product>.Fail(Error.Conflict($"Product with SKU '{command.SKU}' already exists."));

            var product = new Product()
            {
                Name = command.Name,
                SKU = command.SKU,
                Price = command.Price,
                StockQuantity = command.StockQuantity
            };

            await _productWriteRepository.AddProductAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Product>.Ok(product);
        }
    }
}
