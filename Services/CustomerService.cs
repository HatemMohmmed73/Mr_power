using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MR_power.Models;
using MR_power.Repositories.Interfaces;
using MR_power.Services.Interfaces;
using MR_power.DTOs;

namespace MR_power.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository customerRepository,
            ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid customer ID", nameof(id));

            try
            {
                return await _customerRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with ID {Id}", id);
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number cannot be empty", nameof(phone));

            try
            {
                return await _customerRepository.GetCustomerByPhoneAsync(phone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with phone {Phone}", phone);
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            try
            {
                return await _customerRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                throw;
            }
        }

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
        {
            try
            {
                return await _customerRepository.SearchCustomersAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<Customer> CreateCustomerAsync(CreateCustomerDTO customerDto)
        {
            if (customerDto == null)
                throw new ArgumentNullException(nameof(customerDto));

            if (!await IsEmailAvailableAsync(customerDto.Email))
                throw new InvalidOperationException($"Email {customerDto.Email} is already in use");

            if (!await IsPhoneAvailableAsync(customerDto.Phone))
                throw new InvalidOperationException($"Phone number {customerDto.Phone} is already in use");

            try
            {
                var customer = new Customer
                {
                    Name = customerDto.Name,
                    Phone = customerDto.Phone,
                    Email = customerDto.Email,
                    Address = customerDto.Address,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                return await _customerRepository.CreateAsync(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                throw;
            }
        }

        public async Task<Customer> UpdateCustomerAsync(int id, UpdateCustomerDTO customerDto)
        {
            if (customerDto == null)
                throw new ArgumentNullException(nameof(customerDto));

            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                    throw new ArgumentException($"Customer with ID {id} not found");

                if (!string.IsNullOrEmpty(customerDto.Email) && customerDto.Email != existingCustomer.Email)
                {
                    if (!await IsEmailAvailableAsync(customerDto.Email))
                        throw new InvalidOperationException($"Email {customerDto.Email} is already in use");
                    existingCustomer.Email = customerDto.Email;
                }

                if (!string.IsNullOrEmpty(customerDto.Phone) && customerDto.Phone != existingCustomer.Phone)
                {
                    if (!await IsPhoneAvailableAsync(customerDto.Phone))
                        throw new InvalidOperationException($"Phone number {customerDto.Phone} is already in use");
                    existingCustomer.Phone = customerDto.Phone;
                }

                if (!string.IsNullOrEmpty(customerDto.Name))
                    existingCustomer.Name = customerDto.Name;

                if (!string.IsNullOrEmpty(customerDto.Address))
                    existingCustomer.Address = customerDto.Address;

                existingCustomer.UpdatedAt = DateTime.UtcNow;

                return await _customerRepository.UpdateAsync(existingCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteCustomerAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid customer ID", nameof(id));

            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                    throw new ArgumentException($"Customer with ID {id} not found");

                await _customerRepository.DeleteAsync(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone number cannot be empty", nameof(phone));

            try
            {
                return await _customerRepository.PhoneExistsAsync(phone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if phone exists: {Phone}", phone);
                throw;
            }
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return await _customerRepository.IsEmailAvailableAsync(email);
        }

        public async Task<bool> IsPhoneAvailableAsync(string phone)
        {
            return await _customerRepository.IsPhoneAvailableAsync(phone);
        }

        public async Task<Customer> GetCustomerByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            try
            {
                return await _customerRepository.GetByEmailAsync(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with email {Email}", email);
                throw;
            }
        }
    }
} 