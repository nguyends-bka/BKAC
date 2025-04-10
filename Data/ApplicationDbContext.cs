using Microsoft.EntityFrameworkCore;
using BKAC.Models;

namespace BKAC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Permission> Permissions { get; set; }  // Thêm DbSet cho Permission

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Thiết lập khóa chính cho History
            modelBuilder.Entity<History>()
                .HasKey(h => h.HistId);

            // Thiết lập khóa chính cho Permission
            modelBuilder.Entity<Permission>()
                .HasKey(p => p.Id);  // Đảm bảo rằng Id là khóa chính

            base.OnModelCreating(modelBuilder);
        }
    }
}
