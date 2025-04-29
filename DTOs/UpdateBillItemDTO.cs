using System.ComponentModel.DataAnnotations;

namespace MR_power.DTOs
{
    public class UpdateBillItemDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int StockItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal Discount { get; set; }
    }
} 