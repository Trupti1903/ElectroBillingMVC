using System.ComponentModel.DataAnnotations;

namespace ElectroBillingMVC.Models
{
    public class Manager
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}