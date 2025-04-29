using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MR_power.Data;
using MR_power.Models;
using MR_power.Repositories.Interfaces;

namespace MR_power.Repositories
{
    public class BillRepository : IBillRepository
    {
        private readonly ApplicationDbContext _context;

        public BillRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Bill> GetByIdAsync(int id)
        {
            return await _context.Bills.FindAsync(id);
        }

        public async Task<Bill> GetBillWithDetailsAsync(int id)
        {
            return await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.User)
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Bill>> GetAllAsync()
        {
            return await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.User)
                .Include(b => b.Items)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> FindAsync(Expression<Func<Bill, bool>> predicate)
        {
            return await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.User)
                .Include(b => b.Items)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<Bill> AddAsync(Bill entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.Bills.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Bill> UpdateAsync(Bill entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Bills.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(Bill entity)
        {
            _context.Bills.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Bills.AnyAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Bill>> GetBillsByCustomerAsync(int customerId)
        {
            return await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.User)
                .Include(b => b.Items)
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetBillsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.User)
                .Include(b => b.Items)
                .Where(b => b.Date >= startDate && b.Date <= endDate)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> SearchBillsAsync(string searchTerm)
        {
            return await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.User)
                .Include(b => b.Items)
                .Where(b => b.BillNumber.Contains(searchTerm) || b.Customer.Name.Contains(searchTerm))
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bill> GetBillByNumberAsync(string billNumber)
        {
            return await _context.Bills
                .Include(b => b.Customer)
                .Include(b => b.User)
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.BillNumber == billNumber);
        }

        public async Task<string> GenerateUniqueBillNumberAsync()
        {
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var lastBill = await _context.Bills
                .Where(b => b.BillNumber.StartsWith(date))
                .OrderByDescending(b => b.BillNumber)
                .FirstOrDefaultAsync();

            if (lastBill == null)
            {
                return $"{date}-001";
            }

            var lastNumber = int.Parse(lastBill.BillNumber.Split('-')[1]);
            return $"{date}-{(lastNumber + 1):D3}";
        }

        public async Task<decimal> CalculateTotalSalesAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Bills
                .Where(b => b.Date >= startDate && b.Date <= endDate)
                .SumAsync(b => b.TotalAmount);
        }
    }
} 