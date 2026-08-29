using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using MediatR;

namespace ECommerce.Application.CQRS.Customers.Queries.GetById
{
    public record GetByIdQuery(int customerId): IRequest<Result<Customer>>;
}
