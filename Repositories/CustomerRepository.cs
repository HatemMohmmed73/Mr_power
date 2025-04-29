using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MR_power.Data;
using MR_power.Models;
using MR_power.Repositories.Interfaces;

namespace MR_power.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Bills)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Customer> GetByEmailAsync(string email)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Customer> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phoneNumber);
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            customer.CreatedAt = DateTime.UtcNow;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateAsync(Customer customer)
        {
            customer.UpdatedAt = DateTime.UtcNow;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.Bills)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
        {
            return await _context.Customers
                .Include(c => c.Bills)
                .Where(c => c.Name.Contains(searchTerm) || 
                           c.Phone.Contains(searchTerm) || 
                           c.Email.Contains(searchTerm))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _context.Customers.AnyAsync(c => c.Email == email);
        }

        public async Task<bool> IsPhoneAvailableAsync(string phone)
        {
            return !await _context.Customers.AnyAsync(c => c.Phone == phone);
        }

        public async Task<Customer> GetCustomerByPhoneAsync(string phone)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Customers.AnyAsync(c => c.Phone == phone);
        }

        public async Task<bool> HasBillsAsync(int customerId)
        {
            return await _context.Bills.AnyAsync(b => b.CustomerId == customerId);
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            return await AddAsync(customer);
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);
            if (customer != null)
            {
                await DeleteAsync(customer);
            }
        }

        public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
        {
            return await SearchCustomersAsync(searchTerm);
        }
    }
}