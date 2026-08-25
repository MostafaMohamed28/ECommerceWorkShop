using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;

namespace ECommerce.Domain.Contract
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default);

        Task<bool> EmailIsExistAsync(string email, CancellationToken ct = default);
        Task AddAsync(Customer customer, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);

    }
}
