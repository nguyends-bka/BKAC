using Microsoft.EntityFrameworkCore;
using BKAC.Models;

namespace BKAC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<History> Histories { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<History>()
                .HasKey(h => h.HistId);  // Đảm bảo rằng HistId là khóa chính

            base.OnModelCreating(modelBuilder);
        }
    }
}
