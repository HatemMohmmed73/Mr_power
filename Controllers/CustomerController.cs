using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MR_power.DTOs;
using MR_power.Services.Interfaces;

namespace MR_power.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerService customerService,
            ILogger<CustomerController> logger)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDTO>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                if (customer == null)
                    return NotFound($"Customer with ID {id} not found");

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the customer");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                return StatusCode(500, "An error occurred while retrieving customers");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> SearchCustomers([FromQuery] string searchTerm)
        {
            try
            {
                var customers = await _customerService.SearchCustomersAsync(searchTerm);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching customers");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<CustomerDTO>> CreateCustomer([FromBody] CreateCustomerDTO createCustomerDto)
        {
            try
            {
                var customer = await _customerService.CreateCustomerAsync(createCustomerDto);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, "An error occurred while creating the customer");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomerDTO>> UpdateCustomer(int id, [FromBody] UpdateCustomerDTO updateCustomerDto)
        {
            try
            {
                if (id != updateCustomerDto.Id)
                    return BadRequest("ID mismatch");

                var customer = await _customerService.UpdateCustomerAsync(updateCustomerDto);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the customer");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                if (!result)
                    return NotFound($"Customer with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the customer");
            }
        }
    }
} 