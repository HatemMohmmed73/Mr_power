using MR_power.Models;

namespace MR_power.Repositories
{
    public interface IUserRepository
    {
        Task<UserAccount> GetByIdAsync(int id);
        Task<UserAccount> GetByUsernameAsync(string username);
        Task<UserAccount> CreateAsync(UserAccount user);
        Task<UserAccount> UpdateAsync(UserAccount user);
        Task DeleteAsync(int id);
        Task<IEnumerable<UserAccount>> GetAllAsync();
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
    }
} 