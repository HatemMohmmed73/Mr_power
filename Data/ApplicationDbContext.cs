using Microsoft.EntityFrameworkCore;
using MR_power.Models;

namespace MR_power.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItem> BillItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Bill configuration
            modelBuilder.Entity<Bill>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bills)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bills)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // BillItem configuration
            modelBuilder.Entity<BillItem>()
                .HasOne(bi => bi.Bill)
                .WithMany(b => b.Items)
                .HasForeignKey(bi => bi.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BillItem>()
                .HasOne(bi => bi.StockItem)
                .WithMany(si => si.BillItems)
                .HasForeignKey(bi => bi.StockItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer configuration
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Phone)
                .IsUnique();

            // StockItem configuration
            modelBuilder.Entity<StockItem>()
                .HasIndex(si => si.Sku)
                .IsUnique();

            // UserAccount configuration
            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Bill>()
                .Property(b => b.Discount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<StockItem>()
                .Property(si => si.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BillItem>()
                .Property(bi => bi.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BillItem>()
                .Property(bi => bi.DiscountPercentage)
                .HasColumnType("decimal(18,2)");
        }
    }
} 