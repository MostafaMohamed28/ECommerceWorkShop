using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.DTOs.BasktDtos;
using MediatR;

namespace ECommerce.Application.CQRS.Baskts.Queries.GetBasketByCustomerId
{
    public record GetBasketByCustomerIdQuery(int CustomerId):IRequest<Result<BasketDto?>>;   
}
