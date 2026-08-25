using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer customer, CancellationToken ct = default)
        {
           await _context.Customers.AddAsync(customer, ct);
        }

        public async  Task<bool> EmailIsExistAsync(string email, CancellationToken ct = default)
        {
           return await _context.Customers.AnyAsync(c => c.Email.ToLower() == email.ToLower(), ct);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken ct = default)
        {
           return await _context.Customers.ToListAsync(ct);
        }

        public async Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
