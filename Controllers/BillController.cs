using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MR_power.DTOs;
using MR_power.Services.Interfaces;

namespace MR_power.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;
        private readonly ILogger<BillController> _logger;

        public BillController(IBillService billService, ILogger<BillController> logger)
        {
            _billService = billService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BillDTO>> GetBillById(int id)
        {
            try
            {
                var bill = await _billService.GetBillByIdAsync(id);
                if (bill == null)
                    return NotFound();

                return Ok(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill by ID: {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the bill.");
            }
        }

        [HttpGet("number/{billNumber}")]
        public async Task<ActionResult<BillDTO>> GetBillByNumber(string billNumber)
        {
            try
            {
                var bill = await _billService.GetBillByNumberAsync(billNumber);
                if (bill == null)
                    return NotFound();

                return Ok(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill by number: {BillNumber}", billNumber);
                return StatusCode(500, "An error occurred while retrieving the bill.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDTO>>> GetAllBills()
        {
            try
            {
                var bills = await _billService.GetAllBillsAsync();
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all bills");
                return StatusCode(500, "An error occurred while retrieving the bills.");
            }
        }

        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<BillDTO>>> GetBillsByCustomer(int customerId)
        {
            try
            {
                var bills = await _billService.GetBillsByCustomerAsync(customerId);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bills for customer: {CustomerId}", customerId);
                return StatusCode(500, "An error occurred while retrieving the bills.");
            }
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<BillDTO>>> GetBillsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var bills = await _billService.GetBillsByDateRangeAsync(startDate, endDate);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bills by date range: {StartDate} to {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while retrieving the bills.");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BillDTO>>> SearchBills([FromQuery] string searchTerm)
        {
            try
            {
                var bills = await _billService.SearchBillsAsync(searchTerm);
                return Ok(bills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching bills: {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching the bills.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<BillDTO>> CreateBill([FromBody] CreateBillDTO createBillDto)
        {
            try
            {
                var bill = await _billService.CreateBillAsync(createBillDto);
                return CreatedAtAction(nameof(GetBillById), new { id = bill.Id }, bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill");
                return StatusCode(500, "An error occurred while creating the bill.");
            }
        }

        [HttpPut]
        public async Task<ActionResult<BillDTO>> UpdateBill([FromBody] UpdateBillDTO updateBillDto)
        {
            try
            {
                var bill = await _billService.UpdateBillAsync(updateBillDto);
                return Ok(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill: {Id}", updateBillDto.Id);
                return StatusCode(500, "An error occurred while updating the bill.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBill(int id)
        {
            try
            {
                await _billService.DeleteBillAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill: {Id}", id);
                return StatusCode(500, "An error occurred while deleting the bill.");
            }
        }

        [HttpGet("total-sales")]
        public async Task<ActionResult<decimal>> CalculateTotalSales([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var totalSales = await _billService.CalculateTotalSalesAsync(startDate, endDate);
                return Ok(totalSales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total sales: {StartDate} to {EndDate}", startDate, endDate);
                return StatusCode(500, "An error occurred while calculating total sales.");
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<ActionResult> GenerateBillPdf(int id)
        {
            try
            {
                var pdfBytes = await _billService.GenerateBillPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"bill-{id}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for bill: {Id}", id);
                return StatusCode(500, "An error occurred while generating the PDF.");
            }
        }
    }
} 