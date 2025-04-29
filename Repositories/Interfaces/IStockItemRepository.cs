using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MR_power.Models;

namespace MR_power.Repositories.Interfaces
{
    public interface IStockItemRepository : IBaseRepository<StockItem>
    {
        Task<StockItem> GetBySkuAsync(string sku);
        Task<IEnumerable<StockItem>> GetLowStockItemsAsync(int threshold);
        Task<IEnumerable<StockItem>> SearchAsync(string searchTerm);
        Task<IEnumerable<StockItem>> GetByCategoryAsync(string category);
        Task<IEnumerable<StockItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
} 