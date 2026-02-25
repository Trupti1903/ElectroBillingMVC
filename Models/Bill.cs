using System;
using System.ComponentModel.DataAnnotations;

namespace ElectroBillingMVC.Models
{
    public class Bill
    {
        public int BillId { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public DateTime BillDate { get; set; }

        public string Status { get; set; }
    }
}