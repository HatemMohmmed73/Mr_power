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
    public class StockItemController : ControllerBase
    {
        private readonly IStockItemService _stockItemService;
        private readonly ILogger<StockItemController> _logger;

        public StockItemController(
            IStockItemService stockItemService,
            ILogger<StockItemController> logger)
        {
            _stockItemService = stockItemService ?? throw new ArgumentNullException(nameof(stockItemService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StockItemDTO>> GetStockItemById(int id)
        {
            try
            {
                var stockItem = await _stockItemService.GetStockItemByIdAsync(id);
                if (stockItem == null)
                    return NotFound($"Stock item with ID {id} not found");

                return Ok(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock item with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the stock item");
            }
        }

        [HttpGet("sku/{sku}")]
        public async Task<ActionResult<StockItemDTO>> GetStockItemBySku(string sku)
        {
            try
            {
                var stockItem = await _stockItemService.GetStockItemBySkuAsync(sku);
                if (stockItem == null)
                    return NotFound($"Stock item with SKU {sku} not found");

                return Ok(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock item with SKU {Sku}", sku);
                return StatusCode(500, "An error occurred while retrieving the stock item");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockItemDTO>>> GetAllStockItems()
        {
            try
            {
                var stockItems = await _stockItemService.GetAllStockItemsAsync();
                return Ok(stockItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stock items");
                return StatusCode(500, "An error occurred while retrieving stock items");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<StockItemDTO>>> GetStockItemsByCategory(string category)
        {
            try
            {
                var stockItems = await _stockItemService.GetStockItemsByCategoryAsync(category);
                return Ok(stockItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock items by category {Category}", category);
                return StatusCode(500, "An error occurred while retrieving stock items");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<StockItemDTO>>> SearchStockItems([FromQuery] string searchTerm)
        {
            try
            {
                var stockItems = await _stockItemService.SearchStockItemsAsync(searchTerm);
                return Ok(stockItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stock items with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while searching stock items");
            }
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<StockItemDTO>>> GetLowStockItems([FromQuery] int threshold)
        {
            try
            {
                var stockItems = await _stockItemService.GetLowStockItemsAsync(threshold);
                return Ok(stockItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock items with threshold {Threshold}", threshold);
                return StatusCode(500, "An error occurred while retrieving low stock items");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<StockItemDTO>> CreateStockItem([FromBody] CreateStockItemDTO createStockItemDto)
        {
            try
            {
                var stockItem = await _stockItemService.CreateStockItemAsync(createStockItemDto);
                return CreatedAtAction(nameof(GetStockItemById), new { id = stockItem.Id }, stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock item");
                return StatusCode(500, "An error occurred while creating the stock item");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<StockItemDTO>> UpdateStockItem(int id, [FromBody] UpdateStockItemDTO updateStockItemDto)
        {
            try
            {
                if (id != updateStockItemDto.Id)
                    return BadRequest("ID mismatch");

                var stockItem = await _stockItemService.UpdateStockItemAsync(updateStockItemDto);
                return Ok(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock item with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the stock item");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStockItem(int id)
        {
            try
            {
                var result = await _stockItemService.DeleteStockItemAsync(id);
                if (!result)
                    return NotFound($"Stock item with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock item with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the stock item");
            }
        }
    }
} 