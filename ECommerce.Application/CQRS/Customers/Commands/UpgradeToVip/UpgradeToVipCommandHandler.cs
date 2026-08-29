using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Customers.Commands.UpgradeToVip
{
    public class UpgradeToVipCommandHandler:IRequestHandler<UpgradeToVipCommand, Result>
    {
        private readonly ICustomerReadRepository _customerReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpgradeToVipCommandHandler(ICustomerReadRepository customerReadRepository, IUnitOfWork unitOfWork)
        {
            _customerReadRepository = customerReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpgradeToVipCommand request, CancellationToken ct = default)
        {
            var customer = await _customerReadRepository.GetByIdAsync(request.customerId, ct);

            if (customer == null)
                return Result.Fail(Error.NotFound("Customer.NotFound", "Customer not found."));

            var totalSpent = customer.Orders
                .Where(o => o.Status == OrderStatus.Paid)
                .Sum(o => o.TotalAmount);

            if (totalSpent < 500m)
            {
                return Result.Fail(new Error("Customer.NotQualified", $"Customer does not qualify for VIP. Total spend {totalSpent:C} is less than required $500.00"));
            }

            customer.IsVip = true;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

    }
}
