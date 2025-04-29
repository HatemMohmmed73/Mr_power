using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MR_power.Models;
using MR_power.DTOs;

namespace MR_power.Services.Interfaces
{
    public interface IBillService
    {
        Task<Bill?> GetBillByIdAsync(int id);
        Task<Bill?> GetBillByNumberAsync(string billNumber);
        Task<IEnumerable<Bill>> GetAllBillsAsync();
        Task<IEnumerable<Bill>> GetBillsByCustomerAsync(int customerId);
        Task<IEnumerable<Bill>> GetBillsByUserAsync(int userId);
        Task<IEnumerable<Bill>> SearchBillsAsync(string searchTerm);
        Task<Bill> CreateBillAsync(CreateBillDTO billDto);
        Task<Bill> UpdateBillAsync(int id, UpdateBillDTO billDto);
        Task DeleteBillAsync(int id);
        Task<byte[]> GenerateBillPdfAsync(int id);
        Task<decimal> CalculateTotalSalesAsync(DateTime? startDate, DateTime? endDate);
    }
} 