using AutoMapper;
using MR_power.Data;
using MR_power.DTOs;
using MR_power.Exceptions;
using MR_power.Repositories.Interfaces;
using MR_power.Services.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;
using iText.Layout.Properties;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MR_power.Models;
using Microsoft.Extensions.Logging;
using MR_power.Utils;
using MR_power.Repositories;

namespace MR_power.Services
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly Repositories.Interfaces.ICustomerRepository _customerRepository;
        private readonly Repositories.Interfaces.IUserRepository _userRepository;
        private readonly Repositories.Interfaces.IStockItemRepository _stockItemRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BillService> _logger;

        public BillService(
            IBillRepository billRepository,
            Repositories.Interfaces.ICustomerRepository customerRepository,
            Repositories.Interfaces.IUserRepository userRepository,
            Repositories.Interfaces.IStockItemRepository stockItemRepository,
            IMapper mapper,
            ILogger<BillService> logger)
        {
            _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _stockItemRepository = stockItemRepository ?? throw new ArgumentNullException(nameof(stockItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Bill> GetBillByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid bill ID", nameof(id));

            try
            {
                return await _billRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill with ID {Id}", id);
                throw;
            }
        }

        public async Task<Bill> GetBillByNumberAsync(string billNumber)
        {
            if (string.IsNullOrWhiteSpace(billNumber))
                throw new ArgumentException("Bill number cannot be empty", nameof(billNumber));

            try
            {
                return await _billRepository.GetBillByNumberAsync(billNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill with number {BillNumber}", billNumber);
                throw;
            }
        }

        public async Task<IEnumerable<Bill>> GetBillsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before or equal to end date");

            try
            {
                return await _billRepository.GetBillsByDateRangeAsync(startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bills between {StartDate} and {EndDate}", startDate, endDate);
                throw;
            }
        }

        public async Task<IEnumerable<Bill>> GetBillsByUserAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentException($"User with ID {userId} not found");

                return await _billRepository.FindAsync(b => b.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bills for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<Bill>> GetBillsByCustomerAsync(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("Invalid customer ID", nameof(customerId));

            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);
                if (customer == null)
                    throw new ArgumentException($"Customer with ID {customerId} not found");

                return await _billRepository.GetBillsByCustomerAsync(customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bills for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<decimal> CalculateBillTotalAsync(int billId)
        {
            if (billId <= 0)
                throw new ArgumentException("Invalid bill ID", nameof(billId));

            try
            {
                var bill = await _billRepository.GetBillWithDetailsAsync(billId);
                if (bill == null)
                    throw new ArgumentException($"Bill with ID {billId} not found");

                decimal total = 0;
                foreach (var item in bill.Items)
                {
                    total += item.Quantity * item.UnitPrice;
                }

                return total - bill.Discount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total for bill {BillId}", billId);
                throw;
            }
        }

        public async Task<Bill> CreateBillAsync(CreateBillDTO billDto)
        {
            if (billDto == null)
                throw new ArgumentNullException(nameof(billDto));

            try
            {
                var customer = await _customerRepository.GetByIdAsync(billDto.CustomerId);
                if (customer == null)
                    throw new ArgumentException($"Customer with ID {billDto.CustomerId} not found");

                var user = await _userRepository.GetByIdAsync(billDto.UserId);
                if (user == null)
                    throw new ArgumentException($"User with ID {billDto.UserId} not found");

                var bill = new Bill
                {
                    CustomerId = billDto.CustomerId,
                    UserId = billDto.UserId,
                    BillNumber = await _billRepository.GenerateUniqueBillNumberAsync(),
                    Status = "Created",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    BillDate = DateTime.UtcNow,
                    Discount = billDto.Discount,
                    Model = billDto.Model,
                    SerialNumber = billDto.SerialNumber
                };

                return await _billRepository.AddAsync(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill");
                throw;
            }
        }

        public async Task<Bill> UpdateBillAsync(int id, UpdateBillDTO billDto)
        {
            if (billDto == null)
                throw new ArgumentNullException(nameof(billDto));

            try
            {
                var bill = await _billRepository.GetByIdAsync(id);
                if (bill == null)
                    throw new ArgumentException($"Bill with ID {id} not found");

                bill.Model = billDto.Model ?? bill.Model;
                bill.SerialNumber = billDto.SerialNumber ?? bill.SerialNumber;
                bill.Discount = billDto.Discount ?? bill.Discount;
                bill.UpdatedAt = DateTime.UtcNow;

                return await _billRepository.UpdateAsync(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateBillStatusAsync(int id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be empty", nameof(status));

            try
            {
                var bill = await _billRepository.GetByIdAsync(id);
                if (bill == null)
                    throw new ArgumentException($"Bill with ID {id} not found");

                bill.Status = status;
                bill.UpdatedAt = DateTime.UtcNow;

                await _billRepository.UpdateAsync(bill);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for bill {Id}", id);
                return false;
            }
        }

        public async Task DeleteBillAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid bill ID", nameof(id));

            try
            {
                var bill = await _billRepository.GetByIdAsync(id);
                if (bill == null)
                    throw new ArgumentException($"Bill with ID {id} not found");

                await _billRepository.DeleteAsync(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill with ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Bill>> GetAllBillsAsync()
        {
            try
            {
                return await _billRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bills");
                throw;
            }
        }

        public async Task<IEnumerable<Bill>> SearchBillsAsync(string searchTerm)
        {
            try
            {
                return await _billRepository.SearchBillsAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching bills with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<byte[]> GenerateBillPdfAsync(int id)
        {
            try
            {
                var bill = await _billRepository.GetByIdAsync(id);
                if (bill == null)
                {
                    throw new ArgumentException($"Bill with ID {id} not found");
                }

                // TODO: Implement PDF generation logic
                throw new NotImplementedException("PDF generation not implemented yet");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bill PDF: {Id}", id);
                throw;
            }
        }

        public async Task<decimal> CalculateTotalSalesAsync(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                return await _billRepository.CalculateTotalSalesAsync(startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total sales");
                throw;
            }
        }
    }
} 