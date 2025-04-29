using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MR_power.Models;

namespace MR_power.Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<UserAccount>
    {
        Task<UserAccount> GetByUsernameAsync(string username);
        Task<UserAccount> GetByEmailAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<IEnumerable<UserAccount>> GetAllActiveUsersAsync();
    }
} 