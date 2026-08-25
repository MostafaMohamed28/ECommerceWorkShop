using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Dtos
{
    public class OrderItemRequestDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public List<OrderItemRequestDto> Items { get; set; } = new();
        public string? CouponCode { get; set; }
    }
}
