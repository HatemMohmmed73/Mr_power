using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MR_power.Models;

namespace MR_power.Data;

public partial class MrPower1Context : DbContext
{
    public MrPower1Context()
    {
    }

    public MrPower1Context(DbContextOptions<MrPower1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }
    public virtual DbSet<BillItem> BillItems { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<StockItem> StockItems { get; set; }
    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DB1"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.BillNumber)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Model)
                  .HasMaxLength(100)
                  .HasDefaultValue("N/A");

            entity.Property(e => e.SerialNumber)
                  .HasMaxLength(100)
                  .HasDefaultValue("N/A");

            entity.Property(e => e.TotalAmount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Discount)
                  .HasColumnType("decimal(18,2)")
                  .HasDefaultValue(0);

            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasMaxLength(20)
                  .HasDefaultValue("Pending");

            entity.HasOne(d => d.Customer)
                  .WithMany(p => p.Bills)
                  .HasForeignKey(d => d.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.User)
                  .WithMany(p => p.Bills)
                  .HasForeignKey(d => d.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BillItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Quantity)
                  .IsRequired();

            entity.Property(e => e.Price)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(e => e.Total)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.HasOne(d => d.Bill)
                  .WithMany(p => p.Items)
                  .HasForeignKey(d => d.BillId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.StockItem)
                  .WithMany()
                  .HasForeignKey(d => d.StockItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Phone)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(e => e.Email)
                  .HasMaxLength(100);

            entity.Property(e => e.Address)
                  .HasMaxLength(200);

            entity.HasIndex(e => e.Phone)
                  .IsUnique();
        });

        modelBuilder.Entity<StockItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Sku)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Description)
                  .HasMaxLength(500);

            entity.Property(e => e.Category)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Price)
                  .HasColumnType("decimal(18,2)")
                  .IsRequired();

            entity.Property(e => e.Quantity)
                  .IsRequired();

            entity.HasIndex(e => e.Sku)
                  .IsUnique();
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Username)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Email)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Password)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.Role)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(e => e.IsActive)
                  .HasDefaultValue(true);

            entity.HasIndex(e => e.Username)
                  .IsUnique();

            entity.HasIndex(e => e.Email)
                  .IsUnique();
        });
    }
}
