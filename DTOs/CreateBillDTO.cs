using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MR_power.DTOs
{
    public class CreateBillDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateBillItemDTO> Items { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal Discount { get; set; }

        [StringLength(100)]
        public string Model { get; set; }

        [StringLength(100)]
        public string SerialNumber { get; set; }
    }
} 