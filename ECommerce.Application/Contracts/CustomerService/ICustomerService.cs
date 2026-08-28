using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;

namespace ECommerce.Application.Contracts.CustomerService
{
    public interface ICustomerService
    {
        Task<Result<Customer>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<Customer>> CreateCustomerAsync(CreateCustomerDto customerDto, CancellationToken ct = default);
        Task<Result> UpgradeToVipAsync(int customerId, CancellationToken ct = default);
    }
}
