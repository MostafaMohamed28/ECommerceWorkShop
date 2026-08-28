using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;

namespace ECommerce.Application.Contracts.ProductService
{
    public interface IProductService
    {
        Task<Result<Product>> GetProductByIdAsync(int id, CancellationToken ct = default);
        Task<Result<IReadOnlyList<Product>>> GetAllProductsAsync(CancellationToken ct = default);
        Task<Result<Product>> CreateProductAsync(CreateProductDto dto, CancellationToken ct = default);
        Task<Result<Product>> UpdateProductAsync(int id, UpdateProductDto dto, CancellationToken ct = default);
        Task<Result> DeleteProductAsync(int id, CancellationToken ct = default);
    }
}
