using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.Basket;

namespace ECommerce.Domain.Contract.Baskets
{
    public interface IBasketReadRepository
    {
        Task<Basket?> GetBasketByIdAsync(int basketId,CancellationToken ct=default);//read
        Task<Basket?> GetBasketByCustomerIdAsync(int customerId,CancellationToken ct=default);//read
    }

    public interface IBasketWriteRepository
    {
        Task<Basket> AddBasketAsync(int CustomerId, CancellationToken ct=default);//create
        Task UpdateBasketAsync(Basket basket, CancellationToken ct=default);//write
        Task DeleteBasketAsync(Basket basket, CancellationToken ct=default);//write
        Task<int> RemoveStaleItemsAsync(DateTime cutoffDate, CancellationToken ct = default);

    }
}
