using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using MR_power.Models;
using MR_power.Repositories.Interfaces;
using MR_power.Services.Interfaces;
using MR_power.DTOs;

namespace MR_power.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UserAccount?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user == null || !VerifyPassword(password, user.PasswordHash))
                    return null;

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user {Username}", username);
                throw;
            }
        }

        public async Task<UserAccount?> GetUserByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID", nameof(id));

            try
            {
                return await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID {Id}", id);
                throw;
            }
        }

        public async Task<UserAccount?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            try
            {
                return await _userRepository.GetByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with username {Username}", username);
                throw;
            }
        }

        public async Task<UserAccount?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            try
            {
                return await _userRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with email {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<UserAccount>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<UserAccount> CreateUserAsync(CreateUserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            try
            {
                if (!await IsUsernameAvailableAsync(userDto.Username))
                    throw new InvalidOperationException($"Username {userDto.Username} is already in use");

                if (!await IsEmailAvailableAsync(userDto.Email))
                    throw new InvalidOperationException($"Email {userDto.Email} is already in use");

                var user = new UserAccount
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    PasswordHash = HashPassword(userDto.Password),
                    Role = userDto.Role,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                return await _userRepository.AddAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<UserAccount> UpdateUserAsync(int id, UpdateUserDTO userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            try
            {
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == null)
                    throw new ArgumentException($"User with ID {id} not found");

                if (userDto.Username != existingUser.Username && 
                    await _userRepository.UsernameExistsAsync(userDto.Username))
                    throw new ArgumentException("Username already exists");

                if (userDto.Email != existingUser.Email && 
                    await _userRepository.EmailExistsAsync(userDto.Email))
                    throw new ArgumentException("Email already exists");

                existingUser.Username = userDto.Username;
                existingUser.Email = userDto.Email;
                existingUser.Role = userDto.Role;
                existingUser.IsActive = userDto.IsActive;
                existingUser.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(userDto.Password))
                {
                    existingUser.Password = userDto.Password; // Note: In production, hash the password
                }

                return await _userRepository.UpdateAsync(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID", nameof(id));

            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new ArgumentException($"User with ID {id} not found");

                await _userRepository.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                return user != null && user.Password == password; // Note: In production, use proper password hashing
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating credentials for user {Username}", username);
                throw;
            }
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            try
            {
                return await _userRepository.UsernameExistsAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if username exists: {Username}", username);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            try
            {
                return await _userRepository.EmailExistsAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if email exists: {Email}", email);
                throw;
            }
        }

        public async Task<IEnumerable<UserAccount>> SearchUsersAsync(string searchTerm)
        {
            try
            {
                return await _userRepository.FindAsync(u => 
                    u.Username.Contains(searchTerm) || 
                    u.Email.Contains(searchTerm) ||
                    u.Role.Contains(searchTerm));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<UserAccount>> GetAllActiveUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllActiveUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active users");
                throw;
            }
        }

        // IBaseService implementation
        public async Task<UserAccount?> GetByIdAsync(int id)
        {
            return await GetUserByIdAsync(id);
        }

        public async Task<IEnumerable<UserAccount>> GetAllAsync()
        {
            return await GetAllUsersAsync();
        }

        public async Task<UserAccount> AddAsync(UserAccount entity)
        {
            var dto = new CreateUserDTO
            {
                Username = entity.Username,
                Email = entity.Email,
                Password = entity.Password,
                Role = entity.Role
            };
            return await CreateUserAsync(dto);
        }

        public async Task<UserAccount> UpdateAsync(UserAccount entity)
        {
            var dto = new UpdateUserDTO
            {
                Username = entity.Username,
                Email = entity.Email,
                Password = entity.Password,
                Role = entity.Role,
                IsActive = entity.IsActive
            };
            return await UpdateUserAsync(entity.Id, dto);
        }

        public async Task DeleteAsync(UserAccount entity)
        {
            await DeleteUserAsync(entity.Id);
        }

        public async Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID", nameof(id));
            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new ArgumentException("Current password cannot be empty", nameof(currentPassword));
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password cannot be empty", nameof(newPassword));

            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new ArgumentException($"User with ID {id} not found");

                if (user.PasswordHash != currentPassword) // In a real app, use proper password hashing
                    return false;

                user.PasswordHash = newPassword; // In a real app, hash the password
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {Id}", id);
                throw;
            }
        }

        public async Task<bool> ResetPasswordAsync(int id, string newPassword)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid user ID", nameof(id));
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("New password cannot be empty", nameof(newPassword));

            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    throw new ArgumentException($"User with ID {id} not found");

                user.PasswordHash = newPassword; // In a real app, hash the password
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {Id}", id);
                throw;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            try
            {
                return await _userRepository.IsUsernameAvailableAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username availability: {Username}", username);
                throw;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            try
            {
                return await _userRepository.IsEmailAvailableAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email availability: {Email}", email);
                throw;
            }
        }
    }
} 