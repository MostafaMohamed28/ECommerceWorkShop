using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;

namespace ECommerce.Domain.Entities.Baskets
{
    public class BasketItem
    {
        public int Id { get; private set; }

        public int BasketId { get; private set; }

        public int ProductId { get; private set; }

        public int Quantity { get; private set; }

        public Product? Product { get; private set; }
        public DateTime AddedAt { get; private set; }
        private BasketItem() { } // EF Core

        public BasketItem(int basketId, int productId, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            BasketId = basketId;
            ProductId = productId;
            Quantity = quantity;
            AddedAt = DateTime.UtcNow;
        }

        public void IncreaseQuantity(int amount) => Quantity += amount;
        public void SetQuantity(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");
            Quantity = quantity;
        }
    }
}
