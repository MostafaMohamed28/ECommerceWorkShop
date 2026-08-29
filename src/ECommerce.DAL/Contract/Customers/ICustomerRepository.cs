using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;

namespace ECommerce.Domain.Contract
{
    public interface ICustomerReadRepository
    {
        Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);//read
        Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default);//read
        Task<bool> EmailIsExistAsync(string email, CancellationToken ct = default);//read
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);//read
    }

    public interface ICustomerWriteRepository
    {
        Task AddAsync(Customer customer, CancellationToken ct = default);//write
    }
}
