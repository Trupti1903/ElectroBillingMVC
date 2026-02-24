using System.Collections.Generic;
namespace ElectroBillingMVC.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public List<Bill>? Bills { get; set; }
    }
}
