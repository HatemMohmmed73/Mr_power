using System.Collections.Generic;
using MR_power.DTOs;

namespace MR_power.ViewModels
{
    public class ControlPanelViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalBills { get; set; }
        public decimal TotalRevenue { get; set; }
        public IEnumerable<CustomerDTO> Customers { get; set; }
    }
} 