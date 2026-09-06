using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Entities.Baskets;

namespace ECommerce.Domain.Entities.Basket
{
    public class Basket
    {
        public int Id { get; set; }
        public int CustomerId { get;  set; }
        public Customer? Customer { get;private set; }
        public string? CouponCode { get;  set; }

        public DateTime UpdatedAt { get;  set; } = DateTime.Now;
        public List<BasketItem> Items { get; set; } = new();
        public IReadOnlyCollection<BasketItem> GetItems() => Items.AsReadOnly();
        private Basket() { } // for EF Core

        public Basket(int customerId)
        {
            CustomerId = customerId;
        }
        public void AddItem(int productId, int quantity)
        {
            var existing = Items.FirstOrDefault(i => i.ProductId == productId);
            if (existing is not null)
                existing.IncreaseQuantity(quantity);
            else
                Items.Add(new BasketItem(Id, productId, quantity)); 
            Touch();
        }

        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
            Touch();
        }

        public void UpdateItemQuantity(int productId, int newQuantity)
        {
            var item = Items.FirstOrDefault(i => i.ProductId == productId);
            if (item is null)
                throw new InvalidOperationException("Item not found in basket");

            if (newQuantity <= 0)
                Items.Remove(item); // لو الكمية 0، امسح المنتج بدل ما تسيبه بكمية غلط
            else
                item.SetQuantity(newQuantity);

            Touch();
        }

        public void ApplyCoupon(string code)
        {
            CouponCode = code;
            Touch();
        }

        private void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
