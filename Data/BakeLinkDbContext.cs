using Bake_Link.Models;
using Microsoft.EntityFrameworkCore;

namespace Bake_Link.Data;

public class BakeLinkDbContext : DbContext
{
    public BakeLinkDbContext(DbContextOptions<BakeLinkDbContext> options)
        : base(options)
    {
    }

    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Bakery> Bakeries => Set<Bakery>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<User> Users => Set<User>();
}
