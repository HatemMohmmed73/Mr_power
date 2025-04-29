using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR_power.Models
{
    public class BillItem : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BillId { get; set; }

        [ForeignKey("BillId")]
        public Bill Bill { get; set; }

        [Required]
        public int StockItemId { get; set; }

        [ForeignKey("StockItemId")]
        public StockItem StockItem { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountPercentage { get; set; }

        [Required]
        public string StockItemName { get; set; }

        [NotMapped]
        public decimal Subtotal => Quantity * UnitPrice;

        [NotMapped]
        public decimal DiscountAmount => Subtotal * (DiscountPercentage / 100);

        [NotMapped]
        public decimal Total => Subtotal - DiscountAmount;
    }
} 