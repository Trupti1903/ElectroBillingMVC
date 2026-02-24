using Microsoft.EntityFrameworkCore;

namespace ElectroBillingMVC.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                   : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Bill> Bills { get; set; }
    }
}
