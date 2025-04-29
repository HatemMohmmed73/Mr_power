using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MR_power.DTOs;
using MR_power.Models;

namespace MR_power.Services.Interfaces
{
    public interface IStockItemService
    {
        Task<StockItem?> GetStockItemByIdAsync(int id);
        Task<StockItem?> GetStockItemBySkuAsync(string sku);
        Task<IEnumerable<StockItem>> GetAllStockItemsAsync();
        Task<IEnumerable<StockItem>> SearchStockItemsAsync(string searchTerm);
        Task<StockItem> CreateStockItemAsync(CreateStockItemDTO stockItemDto);
        Task<StockItem> UpdateStockItemAsync(int id, UpdateStockItemDTO stockItemDto);
        Task DeleteStockItemAsync(int id);
        Task<bool> SkuExistsAsync(string sku);
        Task<bool> UpdateStockQuantityAsync(int id, int quantity);
    }
} 