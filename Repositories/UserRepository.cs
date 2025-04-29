using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MR_power.Data;
using MR_power.Models;
using MR_power.Repositories.Interfaces;

namespace MR_power.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserAccount> GetByIdAsync(int id)
        {
            return await _context.UserAccounts
                .Include(u => u.Bills)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserAccount> GetByUsernameAsync(string username)
        {
            return await _context.UserAccounts
                .Include(u => u.Bills)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserAccount> GetByEmailAsync(string email)
        {
            return await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserAccount> AddAsync(UserAccount user)
        {
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.UserAccounts.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserAccount> UpdateAsync(UserAccount user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.UserAccounts.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(UserAccount user)
        {
            _context.UserAccounts.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserAccount>> GetAllAsync()
        {
            return await _context.UserAccounts
                .Include(u => u.Bills)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserAccount>> FindAsync(Expression<Func<UserAccount, bool>> predicate)
        {
            return await _context.UserAccounts
                .Include(u => u.Bills)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return !await _context.UserAccounts.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _context.UserAccounts.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null)
                return false;

            return user.PasswordHash == password; // In a real application, use proper password hashing
        }

        public async Task<IEnumerable<UserAccount>> GetAllActiveUsersAsync()
        {
            return await _context.UserAccounts
                .Where(u => u.IsActive)
                .Include(u => u.Bills)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<bool> HasBillsAsync(int userId)
        {
            return await _context.Bills.AnyAsync(b => b.UserId == userId);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.UserAccounts.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.UserAccounts.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _context.UserAccounts.AnyAsync(u => u.Username == username);
        }

        public async Task<UserAccount> CreateAsync(UserAccount user)
        {
            return await AddAsync(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                await DeleteAsync(user);
            }
        }
    }
} 