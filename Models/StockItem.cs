using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR_power.Models
{
    public class StockItem : BaseEntity
    {
        [Required]
        public string Sku { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int MinQuantity { get; set; }

        [Required]
        public string Supplier { get; set; }

        [Required]
        public string Location { get; set; }

        public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();
    }
} 