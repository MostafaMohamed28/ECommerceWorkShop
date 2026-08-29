using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler:IRequestHandler<CreateCustomerCommand, Result<Customer>>
    {

        private readonly ICustomerReadRepository _customerReadRepository;
        private readonly ICustomerWriteRepository _customerWriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateCustomerCommandHandler(ICustomerReadRepository customerReadRepository, ICustomerWriteRepository customerWriteRepository, IUnitOfWork unitOfWork)
        {
            _customerReadRepository = customerReadRepository;
            _customerWriteRepository = customerWriteRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Customer>> Handle(CreateCustomerCommand request, CancellationToken ct = default)
        {

            if (string.IsNullOrWhiteSpace(request.customerDto.FullName))
                return Result<Customer>.Fail(Error.Validation("FullName.Validation", "FullName is required."));



            if (string.IsNullOrWhiteSpace(request.customerDto.Email) || !request.customerDto.Email.Contains("@"))
                return Result<Customer>.Fail(Error.Validation("EmailRequired", "Email is required and must be valid."));

            var emailExists = await _customerReadRepository.EmailIsExistAsync(request.customerDto.Email, ct);
            if (emailExists)
                return Result<Customer>.Fail(Error.Validation("EmailExists", "Email already exists."));

            var customer = new Customer
            {
                FullName = request.customerDto.FullName,
                Email = request.customerDto.Email,
                IsVip = request.customerDto.IsVip
            };

            await _customerWriteRepository.AddAsync(customer, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return Result<Customer>.Ok(customer);
        }

    }
}
