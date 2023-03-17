using Microsoft.EntityFrameworkCore;

namespace mini_shop_api.Models
{
    public class MyDbContext: DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; } 
        public DbSet<Product> Products { get; set; } 
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductDocument> ProductDocuments { get; set; }

    }
}
