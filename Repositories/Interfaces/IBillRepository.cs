using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MR_power.Models;

namespace MR_power.Repositories.Interfaces
{
    public interface IBillRepository : IBaseRepository<Bill>
    {
        Task<Bill> GetBillWithDetailsAsync(int id);
        Task<IEnumerable<Bill>> GetBillsByCustomerAsync(int customerId);
        Task<IEnumerable<Bill>> GetBillsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Bill>> SearchBillsAsync(string searchTerm);
        Task<Bill> GetBillByNumberAsync(string billNumber);
        Task<string> GenerateUniqueBillNumberAsync();
        Task<decimal> CalculateTotalSalesAsync(DateTime startDate, DateTime endDate);
    }
} 