using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Contracts.ProductService;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Product>> CreateProductAsync(CreateProductDto dto, CancellationToken ct = default)
        {

            if (dto.Price <= 0)
                return Result<Product>.Fail(Error.Validation("Price must be greater than zero."));


            if (dto.StockQuantity < 0)
                return Result<Product>.Fail(Error.Validation("Stock quantity cannot be negative."));

            
            var skuExists = await _productRepository.ProductExistAsync(dto.SKU, ct);
            if (skuExists)
                return Result<Product>.Fail(Error.Conflict($"Product with SKU '{dto.SKU}' already exists."));

            var product = new Product()
            {
                Name = dto.Name,
                SKU = dto.SKU,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
            };

            await _productRepository.AddProductAsync(product, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Product>.Ok(product);
        }

        public async Task<Result> DeleteProductAsync(int id, CancellationToken ct = default)
        {
            var product =await _productRepository.GetProductForUpdateOrDeleteAsync(id, ct);
            if (product == null)
                return Result.Fail(Error.NotFound("Product.NotFound", $"Product with ID {id} not found."));

            await _productRepository.DeleteProductAsync(product.Id, ct);

            return Result.Ok();
        }

        public async Task<Result<IReadOnlyList<Product>>> GetAllProductsAsync(CancellationToken ct = default)
        {
           var products=await _productRepository.GetAllProductsAsync(ct);

           return Result<IReadOnlyList<Product>>.Ok(products);
        }

        public async Task<Result<Product>> GetProductByIdAsync(int id, CancellationToken ct = default)
        {
            var product =await _productRepository.GetProductByIdAsync(id, ct);
            if (product == null)
            {
                return Result<Product>.Fail(Error.NotFound("Product.NotFound", $"Product with ID {id} not found."));
            }
            return Result<Product>.Ok(product);
        }

        public async Task<Result<Product>> UpdateProductAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
        {
            var productexisting = await _productRepository.GetProductForUpdateOrDeleteAsync(id, ct);
            if (productexisting == null)
                return Result<Product>.Fail(Error.NotFound("Product.NotFound", $"Product with ID {id} not found."));
          
            if (dto.Price <= 0)
                return Result<Product>.Fail(Error.Validation("Price must be positive."));

            productexisting.Name = dto.Name;
            productexisting.SKU = dto.SKU;
            productexisting.Price = dto.Price;
            productexisting.StockQuantity = dto.StockQuantity;

            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Product>.Ok(productexisting);
        }
    }
}
