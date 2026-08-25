using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Contracts;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<Customer>> CreateCustomerAsync(CreateCustomerDto customerDto, CancellationToken ct = default)
        {

            if (string.IsNullOrWhiteSpace(customerDto.FullName))
                return Result<Customer>.Fail(Error.Validation("FullName.Validation", "FullName is required."));



            if (string.IsNullOrWhiteSpace(customerDto.Email) || !customerDto.Email.Contains("@"))
                return Result<Customer>.Fail(Error.Validation("EmailRequired", "Email is required and must be valid."));

            var emailExists = await _customerRepository.EmailIsExistAsync(customerDto.Email, ct);
            if (emailExists)
                return Result<Customer>.Fail(Error.Validation("EmailExists", "Email already exists."));

            var customer = new Customer
            {
                FullName = customerDto.FullName,
                Email = customerDto.Email,
                IsVip = customerDto.IsVip
            };

            await _customerRepository.AddAsync(customer, ct);
            await _customerRepository.SaveChangesAsync(ct);

            return Result<Customer>.Ok(customer);
        }

        public async Task<Result<Customer>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var customer = await _customerRepository.GetByIdAsync(id, ct);

            if (customer == null)
                return Result<Customer>.Fail(new Error("Customer.NotFound", "Customer not found."));

            return Result<Customer>.Ok(customer);
        }

        public async Task<Result> UpgradeToVipAsync(int customerId, CancellationToken ct = default)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId, ct);

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
            await _customerRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }
    }
}
