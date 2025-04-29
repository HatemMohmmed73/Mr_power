using MR_power.Models;
using MR_power.DTOs;

namespace MR_power.Services
{
    public interface IUserService
    {
        Task<UserAccount> GetUserByIdAsync(int id);
        Task<UserAccount> GetUserByUsernameAsync(string username);
        Task<UserAccount> CreateUserAsync(CreateUserDTO userDto);
        Task<UserAccount> UpdateUserAsync(int id, UpdateUserDTO userDto);
        Task DeleteUserAsync(int id);
        Task<UserAccount> AuthenticateAsync(string username, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<IEnumerable<UserAccount>> GetAllUsersAsync();
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
    }
} 