using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;

namespace ECommerce.Domain.Contract.Products
{
    public interface IProductReadRepository
    {
        Task<Product?> GetProductByIdAsync(int id, CancellationToken ct = default);//Read
        Task<bool> ProductExistAsync(string sku, CancellationToken ct = default);//Read
        Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken ct = default);//Read
        Task<Product?> GetProductForUpdateOrDeleteAsync(int id, CancellationToken ct = default);//Read
    }
    public interface IProductWriteRepository
    {
        Task AddProductAsync(Product product, CancellationToken ct = default);//Create
        Task UpdateProductAsync(Product product, CancellationToken ct = default);//Update
        Task DeleteProductAsync(int id, CancellationToken ct = default);//Delete


    }
}
