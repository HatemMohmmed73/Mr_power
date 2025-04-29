using System;
using System.ComponentModel.DataAnnotations;

namespace MR_power.DTOs
{
    public class CustomerDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        public bool IsActive { get; set; }
    }

    public class CreateCustomerDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
    }

    public class UpdateCustomerDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
    }
} 