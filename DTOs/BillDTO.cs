using System;
using System.Collections.Generic;

namespace MR_power.DTOs
{
    public class BillDTO
    {
        public int Id { get; set; }
        public string BillNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerEmail { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime BillDate { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public string Status { get; set; }
        public decimal Discount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public ICollection<BillItemDTO> Items { get; set; }
    }
} 