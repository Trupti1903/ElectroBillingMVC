using Microsoft.EntityFrameworkCore;

namespace ElectroBillingMVC.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Manager> Managers { get; set; }
    }
}