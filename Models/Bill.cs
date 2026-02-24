using System.ComponentModel.DataAnnotations;


namespace ElectroBillingMVC.Models
{
    public class Bill
    {
        public int BillId { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime BillDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public string Status { get; set; } // "Paid" or "Pending"
     }
}
