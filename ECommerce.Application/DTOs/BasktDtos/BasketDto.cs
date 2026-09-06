using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Entities.Baskets;

namespace ECommerce.Application.DTOs.BasktDtos
{
    public record BasketDto
    {
       public int Id { get; init; }
       public int CustomerId { get; init; }
       public string? CouponCode { get; init; }
       public List<BasketItemDto> Items { get; init; } = new();
    }

}
