using Microsoft.EntityFrameworkCore;

namespace mini_shop_api.Models
{
    public class MyDbContext: DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; } 
        public DbSet<Item> Items { get; set; } 
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<SoldProduct> SoldProducts { get; set; }

    }
}
