using Microsoft.EntityFrameworkCore;

namespace OnlineShopV1.DAL
{
    public class OnlineShopDbContext : DbContext
    {
        public OnlineShopDbContext(DbContextOptions<OnlineShopDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var defaultAdmin = new Admin
            {
                ID = 1,
                Username = "omidd",
                Password = "12345"
            };
            defaultAdmin.HashPassword();

            modelBuilder.Entity<Admin>().HasData(defaultAdmin);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Authentication> Authentications { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}