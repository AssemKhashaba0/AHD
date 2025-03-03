using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace AHD.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Mosque> Mosques { get; set; }
    public DbSet<Cemetery> Cemeteries { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<DeliveryAgent> DeliveryAgents { get; set; }
    public DbSet<Recurrence> Recurrences { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> carts { get; set; }
    public DbSet<OrderHistory> OrderHistory { get; set; }
    public DbSet<DeliveryLocation> DeliveryLocations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Order>()
       .HasOne(o => o.DeliveryLocation)
       .WithMany()
       .HasForeignKey(o => o.DeliveryLocationId);
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeliveryAgent>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .Property(o => o.DeliveryManId)
            .IsRequired(false);

       
    }

}
