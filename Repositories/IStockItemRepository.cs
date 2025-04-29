using MR_power.Models;

namespace MR_power.Repositories
{
    public interface IStockItemRepository
    {
        Task<StockItem> GetByIdAsync(int id);
        Task<StockItem> GetBySkuAsync(string sku);
        Task<StockItem> CreateAsync(StockItem stockItem);
        Task<StockItem> UpdateAsync(StockItem stockItem);
        Task DeleteAsync(int id);
        Task<IEnumerable<StockItem>> GetAllAsync();
        Task<IEnumerable<StockItem>> GetByCategoryAsync(string category);
        Task<IEnumerable<StockItem>> GetLowStockItemsAsync();
        Task<bool> UpdateQuantityAsync(int id, int quantity);
        Task<bool> IsSkuAvailableAsync(string sku);
    }
} 