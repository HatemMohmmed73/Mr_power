using MR_power.Models;
using MR_power.DTOs;

namespace MR_power.Services
{
    public interface IStockItemService
    {
        Task<StockItem> GetStockItemByIdAsync(int id);
        Task<StockItem> GetStockItemBySkuAsync(string sku);
        Task<StockItem> CreateStockItemAsync(CreateStockItemDTO stockItemDto);
        Task<StockItem> UpdateStockItemAsync(int id, UpdateStockItemDTO stockItemDto);
        Task DeleteStockItemAsync(int id);
        Task<IEnumerable<StockItem>> GetAllStockItemsAsync();
        Task<IEnumerable<StockItem>> GetStockItemsByCategoryAsync(string category);
        Task<IEnumerable<StockItem>> GetLowStockItemsAsync();
        Task<bool> UpdateStockQuantityAsync(int id, int quantity);
        Task<bool> IsSkuAvailableAsync(string sku);
    }
} 