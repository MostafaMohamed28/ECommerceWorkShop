using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Entities.Baskets.Records
{
    public record AddItemToBasketRequest(int CustomerId, int ProductId, int Quantity);
}
