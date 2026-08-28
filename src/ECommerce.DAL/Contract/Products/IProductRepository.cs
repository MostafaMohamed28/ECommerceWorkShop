using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;

namespace ECommerce.Domain.Contract.Products
{
    public interface IProductRepository
    {
        Task<Product?> GetProductByIdAsync(int id, CancellationToken ct = default);
        Task<bool> ProductExistAsync(string sku, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken ct = default);
        Task<Product?> GetProductForUpdateOrDeleteAsync(int id, CancellationToken ct = default);
        Task AddProductAsync(Product product, CancellationToken ct = default);
        Task DeleteProductAsync(int id, CancellationToken ct = default);


    }
}
