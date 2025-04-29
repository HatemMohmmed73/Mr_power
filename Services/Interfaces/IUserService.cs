using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MR_power.Models;
using MR_power.DTOs;

namespace MR_power.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserAccount?> AuthenticateAsync(string username, string password);
        Task<UserAccount?> GetUserByIdAsync(int id);
        Task<UserAccount?> GetUserByUsernameAsync(string username);
        Task<UserAccount?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserAccount>> GetAllUsersAsync();
        Task<UserAccount> CreateUserAsync(CreateUserDTO userDto);
        Task<UserAccount> UpdateUserAsync(int id, UpdateUserDTO userDto);
        Task DeleteUserAsync(int id);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
} 