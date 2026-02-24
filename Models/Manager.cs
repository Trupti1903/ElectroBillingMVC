using System.ComponentModel.DataAnnotations;

namespace ElectroBillingMVC.Models
{
    public class Manager
    {
        public int ManagerId { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
