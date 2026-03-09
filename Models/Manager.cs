using System.ComponentModel.DataAnnotations;

namespace ElectroBillingMVC.Models
{
    public class Manager
    {
        public int Id { get; set; }

        public string? Name { get; set; }   // Table column = Name

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? ResetToken { get; set; }
    }
}