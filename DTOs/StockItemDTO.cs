using System;
using System.ComponentModel.DataAnnotations;

namespace MR_power.DTOs
{
    public class StockItemDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Sku { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        [StringLength(100)]
        public string Supplier { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 