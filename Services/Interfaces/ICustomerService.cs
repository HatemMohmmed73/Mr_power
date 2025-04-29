using System.Collections.Generic;
using System.Threading.Tasks;
using MR_power.DTOs;
using MR_power.Models;

namespace MR_power.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByPhoneAsync(string phone);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
        Task<Customer> CreateCustomerAsync(CreateCustomerDTO customerDto);
        Task<Customer> UpdateCustomerAsync(int id, UpdateCustomerDTO customerDto);
        Task DeleteCustomerAsync(int id);
        Task<bool> PhoneExistsAsync(string phone);
    }
} 