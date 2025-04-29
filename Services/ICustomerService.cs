using MR_power.Models;
using MR_power.DTOs;

namespace MR_power.Services
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<Customer> CreateCustomerAsync(CreateCustomerDTO customerDto);
        Task<Customer> UpdateCustomerAsync(int id, UpdateCustomerDTO customerDto);
        Task DeleteCustomerAsync(int id);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsPhoneAvailableAsync(string phone);
    }
} 