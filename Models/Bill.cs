using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MR_power.Models
{
    public class Bill : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string BillNumber { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public UserAccount User { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<BillItem> Items { get; set; } = new List<BillItem>();

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        [StringLength(100)]
        public string SerialNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Required]
        public DateTime BillDate { get; set; }

        [NotMapped]
        public decimal MonthlyInterestRate { get; set; }

        [NotMapped]
        public decimal AnnualInterestRate { get; set; }

        [NotMapped]
        public decimal MonthlyInterestAmount { get; set; }

        [NotMapped]
        public decimal AnnualInterestAmount { get; set; }

        [NotMapped]
        public decimal DiscountPercentage { get; set; }

        [NotMapped]
        public decimal DiscountAmount { get; set; }

        [NotMapped]
        public decimal Subtotal => CalculateSubtotal();

        [NotMapped]
        public decimal Total => CalculateTotal();

        private decimal CalculateSubtotal()
        {
            decimal subtotal = 0;
            foreach (var item in Items)
            {
                subtotal += item.Quantity * item.UnitPrice;
            }
            return subtotal;
        }

        private decimal CalculateTotal()
        {
            return Subtotal - Discount;
        }
    }
} 