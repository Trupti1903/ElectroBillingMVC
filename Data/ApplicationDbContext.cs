using Microsoft.EntityFrameworkCore;

namespace ElectroBillingMVC.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Manager> Managers { get; set; }
    }
}