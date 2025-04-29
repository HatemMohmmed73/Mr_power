using MR_power.Models;
using MR_power.DTOs;

namespace MR_power.Services
{
    public interface IBillService
    {
        Task<Bill> GetBillByIdAsync(int id);
        Task<Bill> GetBillByNumberAsync(string billNumber);
        Task<Bill> CreateBillAsync(CreateBillDTO billDto);
        Task<Bill> UpdateBillAsync(int id, UpdateBillDTO billDto);
        Task DeleteBillAsync(int id);
        Task<IEnumerable<Bill>> GetAllBillsAsync();
        Task<IEnumerable<Bill>> GetBillsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Bill>> GetBillsByUserAsync(int userId);
        Task<IEnumerable<Bill>> GetBillsByCustomerAsync(int customerId);
        Task<decimal> CalculateBillTotalAsync(int billId);
        Task<bool> UpdateBillStatusAsync(int billId, string status);
    }
} 