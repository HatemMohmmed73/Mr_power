using MR_power.Models;

namespace MR_power.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> GetByEmailAsync(string email);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsPhoneAvailableAsync(string phone);
    }
} 