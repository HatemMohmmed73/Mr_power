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
    public class StockItemService : IStockItemService
    {
        private readonly IStockItemRepository _stockItemRepository;
        private readonly ILogger<StockItemService> _logger;

        public StockItemService(
            IStockItemRepository stockItemRepository,
            ILogger<StockItemService> logger)
        {
            _stockItemRepository = stockItemRepository;
            _logger = logger;
        }

        public async Task<StockItem> GetByIdAsync(int id)
        {
            try
            {
                return await _stockItemRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock item with ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<StockItem>> GetAllAsync()
        {
            try
            {
                return await _stockItemRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all stock items");
                throw;
            }
        }

        public async Task<StockItem> CreateAsync(StockItem stockItem)
        {
            if (stockItem == null)
                throw new ArgumentNullException(nameof(stockItem));

            try
            {
                return await _stockItemRepository.AddAsync(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock item");
                throw;
            }
        }

        public async Task UpdateAsync(StockItem stockItem)
        {
            if (stockItem == null)
                throw new ArgumentNullException(nameof(stockItem));

            try
            {
                await _stockItemRepository.UpdateAsync(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock item with ID {Id}", stockItem.Id);
                throw;
            }
        }

        public async Task DeleteAsync(StockItem stockItem)
        {
            if (stockItem == null)
                throw new ArgumentNullException(nameof(stockItem));

            try
            {
                await _stockItemRepository.DeleteAsync(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock item with ID {Id}", stockItem.Id);
                throw;
            }
        }

        public async Task<StockItem> GetBySkuAsync(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty", nameof(sku));

            try
            {
                return await _stockItemRepository.GetBySkuAsync(sku);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock item with SKU {Sku}", sku);
                throw;
            }
        }

        public async Task<IEnumerable<StockItem>> GetLowStockItemsAsync(int threshold)
        {
            if (threshold < 0)
                throw new ArgumentException("Threshold cannot be negative", nameof(threshold));

            try
            {
                return await _stockItemRepository.GetLowStockItemsAsync(threshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock items with threshold {Threshold}", threshold);
                throw;
            }
        }

        public async Task<IEnumerable<StockItem>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));

            try
            {
                return await _stockItemRepository.SearchAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stock items with term {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<StockItem>> GetByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));

            try
            {
                return await _stockItemRepository.GetByCategoryAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock items in category {Category}", category);
                throw;
            }
        }

        public async Task<IEnumerable<StockItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0)
                throw new ArgumentException("Minimum price cannot be negative", nameof(minPrice));
            if (maxPrice < minPrice)
                throw new ArgumentException("Maximum price cannot be less than minimum price", nameof(maxPrice));

            try
            {
                return await _stockItemRepository.GetByPriceRangeAsync(minPrice, maxPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock items in price range {MinPrice}-{MaxPrice}", minPrice, maxPrice);
                throw;
            }
        }

        public async Task<StockItem> CreateStockItemAsync(CreateStockItemDTO stockItemDto)
        {
            if (stockItemDto == null)
                throw new ArgumentNullException(nameof(stockItemDto));

            if (!await IsSkuAvailableAsync(stockItemDto.Sku))
                throw new InvalidOperationException($"SKU {stockItemDto.Sku} is already in use");

            try
            {
                var stockItem = new StockItem
                {
                    Sku = stockItemDto.Sku,
                    Name = stockItemDto.Name,
                    Category = stockItemDto.Category,
                    Price = stockItemDto.Price,
                    Quantity = stockItemDto.Quantity,
                    Discount = stockItemDto.Discount,
                    Description = stockItemDto.Description,
                    MinQuantity = stockItemDto.MinQuantity,
                    UnitPrice = stockItemDto.UnitPrice,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                return await _stockItemRepository.AddAsync(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock item");
                throw;
            }
        }

        public async Task<StockItem> UpdateStockItemAsync(int id, UpdateStockItemDTO stockItemDto)
        {
            if (stockItemDto == null)
                throw new ArgumentNullException(nameof(stockItemDto));

            try
            {
                var existingStockItem = await _stockItemRepository.GetByIdAsync(id);
                if (existingStockItem == null)
                    throw new ArgumentException($"Stock item with ID {id} not found");

                if (!string.IsNullOrEmpty(stockItemDto.Sku) && stockItemDto.Sku != existingStockItem.Sku)
                {
                    if (!await IsSkuAvailableAsync(stockItemDto.Sku))
                        throw new InvalidOperationException($"SKU {stockItemDto.Sku} is already in use");
                    existingStockItem.Sku = stockItemDto.Sku;
                }

                if (!string.IsNullOrEmpty(stockItemDto.Name))
                    existingStockItem.Name = stockItemDto.Name;

                if (!string.IsNullOrEmpty(stockItemDto.Category))
                    existingStockItem.Category = stockItemDto.Category;

                if (!string.IsNullOrEmpty(stockItemDto.Description))
                    existingStockItem.Description = stockItemDto.Description;

                if (stockItemDto.Quantity.HasValue)
                    existingStockItem.Quantity = stockItemDto.Quantity.Value;

                if (stockItemDto.MinQuantity.HasValue)
                    existingStockItem.MinQuantity = stockItemDto.MinQuantity.Value;

                if (stockItemDto.UnitPrice.HasValue)
                    existingStockItem.UnitPrice = stockItemDto.UnitPrice.Value;

                existingStockItem.Discount = stockItemDto.Discount;
                existingStockItem.UpdatedAt = DateTime.UtcNow;

                await _stockItemRepository.UpdateAsync(existingStockItem);
                return existingStockItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock item with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteStockItemAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid stock item ID", nameof(id));

            try
            {
                var stockItem = await _stockItemRepository.GetByIdAsync(id);
                if (stockItem == null)
                    throw new ArgumentException($"Stock item with ID {id} not found");

                await _stockItemRepository.DeleteAsync(stockItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting stock item with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> SkuExistsAsync(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty", nameof(sku));

            try
            {
                return await _stockItemRepository.SkuExistsAsync(sku);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if SKU exists: {Sku}", sku);
                throw;
            }
        }

        public async Task<bool> UpdateStockQuantityAsync(int id, int quantity)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid stock item ID", nameof(id));
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

            try
            {
                var stockItem = await _stockItemRepository.GetByIdAsync(id);
                if (stockItem == null)
                    throw new ArgumentException($"Stock item with ID {id} not found");

                stockItem.Quantity = quantity;
                stockItem.UpdatedAt = DateTime.UtcNow;

                await _stockItemRepository.UpdateAsync(stockItem);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock quantity for item with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> IsSkuAvailableAsync(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty", nameof(sku));

            try
            {
                return await _stockItemRepository.IsSkuAvailableAsync(sku);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if SKU is available: {Sku}", sku);
                throw;
            }
        }

        public async Task<IEnumerable<StockItem>> GetStockItemsByCategoryAsync(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));

            try
            {
                return await _stockItemRepository.GetByCategoryAsync(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock items by category: {Category}", category);
                throw;
            }
        }

        public async Task<bool> UpdateQuantityAsync(int id, int quantity)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid stock item ID", nameof(id));
            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

            try
            {
                var stockItem = await _stockItemRepository.GetByIdAsync(id);
                if (stockItem == null)
                    throw new ArgumentException($"Stock item with ID {id} not found");

                stockItem.Quantity = quantity;
                stockItem.UpdatedAt = DateTime.UtcNow;

                await _stockItemRepository.UpdateAsync(stockItem);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock quantity for item with ID {Id}", id);
                throw;
            }
        }
    }
} 