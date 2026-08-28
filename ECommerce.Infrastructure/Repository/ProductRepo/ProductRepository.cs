using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract.Products;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repository.Products
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext appContext;

        public ProductRepository(AppDbContext appContext)
        {
            this.appContext = appContext;
        }   
        public async Task AddProductAsync(Product product, CancellationToken ct = default)
        {
             await appContext.Products.AddAsync(product, ct);
        }

        public async Task DeleteProductAsync(int id, CancellationToken ct = default)
        {
           await appContext.Products.Where(p => p.Id == id).ExecuteDeleteAsync(ct);
        }

        public async Task<IReadOnlyList<Product>> GetAllProductsAsync(CancellationToken ct = default)
        {
            return await appContext.Products.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Product?> GetProductByIdAsync(int id, CancellationToken ct = default)
        {
            var product =await appContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

            return product;
        }

        public async Task<Product?> GetProductForUpdateOrDeleteAsync(int id, CancellationToken ct = default)
        {
            var product =await appContext.Products.AsTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
            return product;
        }

        public async Task<bool> ProductExistAsync(string sku, CancellationToken ct = default)
        {
            return await appContext.Products.AnyAsync(p => p.SKU == sku, ct);
        }

    }
}
