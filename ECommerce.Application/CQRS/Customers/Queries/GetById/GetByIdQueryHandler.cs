using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using MediatR;

namespace ECommerce.Application.CQRS.Customers.Queries.GetById
{
    public class GetByIdQueryHandler:IRequestHandler<GetByIdQuery, Result<Customer>>
    {
        private readonly ICustomerReadRepository _customerReadRepository;
        public GetByIdQueryHandler(ICustomerReadRepository customerReadRepository)
        {
            _customerReadRepository = customerReadRepository;
        }

        public async Task<Result<Customer>> Handle(GetByIdQuery request, CancellationToken ct = default)
        {
            var customer = await _customerReadRepository.GetByIdAsync(request.customerId, ct);

            if (customer == null)
                return Result<Customer>.Fail(new Error("Customer.NotFound", "Customer not found."));

            return Result<Customer>.Ok(customer);
        }
    }
}
