using System;
using System.ComponentModel.DataAnnotations;

namespace MR_power.DTOs
{
    public class BillItemDTO
    {
        public int Id { get; set; }
        public int BillId { get; set; }
        
        [Required]
        public int StockItemId { get; set; }
        
        [StringLength(100, ErrorMessage = "Stock name cannot exceed 100 characters")]
        public string StockItemName { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal UnitPrice { get; set; }
        
        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal DiscountPercentage { get; set; }
        
        public decimal Subtotal { get; set; }
        
        public decimal DiscountAmount { get; set; }
        
        public decimal Total { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 