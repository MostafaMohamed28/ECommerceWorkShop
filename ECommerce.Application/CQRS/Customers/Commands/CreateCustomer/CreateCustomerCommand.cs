using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using MediatR;

namespace ECommerce.Application.CQRS.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(CreateCustomerDto customerDto):IRequest<Result<Customer>>;
}
