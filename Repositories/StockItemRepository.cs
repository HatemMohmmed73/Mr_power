using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MR_power.Data;
using MR_power.Models;
using MR_power.Repositories.Interfaces;

namespace MR_power.Repositories
{
    public class StockItemRepository : IStockItemRepository
    {
        private readonly ApplicationDbContext _context;

        public StockItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StockItem> GetByIdAsync(int id)
        {
            return await _context.StockItems.FindAsync(id);
        }

        public async Task<StockItem> GetBySkuAsync(string sku)
        {
            return await _context.StockItems
                .FirstOrDefaultAsync(s => s.Sku == sku);
        }

        public async Task<IEnumerable<StockItem>> GetAllAsync()
        {
            return await _context.StockItems.ToListAsync();
        }

        public async Task<StockItem> AddAsync(StockItem entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.StockItems.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(StockItem entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.StockItems.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(StockItem entity)
        {
            _context.StockItems.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.StockItems.AnyAsync(s => s.Id == id);
        }

        public async Task<bool> IsSkuAvailableAsync(string sku)
        {
            return !await _context.StockItems.AnyAsync(s => s.Sku == sku);
        }

        public async Task<IEnumerable<StockItem>> GetByCategoryAsync(string category)
        {
            return await _context.StockItems
                .Where(s => s.Category == category)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockItem>> GetLowStockItemsAsync(int threshold)
        {
            return await _context.StockItems
                .Where(s => s.Quantity <= threshold)
                .OrderBy(s => s.Quantity)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockItem>> SearchAsync(string searchTerm)
        {
            return await _context.StockItems
                .Where(s => s.Name.Contains(searchTerm) || 
                           s.Description.Contains(searchTerm) || 
                           s.Sku.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<bool> UpdateQuantityAsync(int id, int quantity)
        {
            var stockItem = await _context.StockItems.FindAsync(id);
            if (stockItem == null)
                return false;

            stockItem.Quantity = quantity;
            stockItem.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StockItem>> FindAsync(Expression<Func<StockItem, bool>> predicate)
        {
            return await _context.StockItems
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IEnumerable<StockItem>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.StockItems
                .Where(s => s.Price >= minPrice && s.Price <= maxPrice)
                .OrderBy(s => s.Price)
                .ToListAsync();
        }

        public async Task<bool> SkuExistsAsync(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new ArgumentException("SKU cannot be empty", nameof(sku));

            return await _context.StockItems
                .AnyAsync(s => s.Sku == sku);
        }

        public async Task<bool> IsStockInUseAsync(int stockId)
        {
            if (stockId <= 0)
                throw new ArgumentException("Invalid stock item ID", nameof(stockId));

            return await _context.BillItems
                .AnyAsync(bi => bi.StockItemId == stockId);
        }

        public async Task<bool> HasSufficientQuantityAsync(int stockId, int quantity)
        {
            if (stockId <= 0)
                throw new ArgumentException("Invalid stock item ID", nameof(stockId));

            if (quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(quantity));

            var stockItem = await _context.StockItems.FindAsync(stockId);
            return stockItem != null && stockItem.Quantity >= quantity;
        }
    }
} 