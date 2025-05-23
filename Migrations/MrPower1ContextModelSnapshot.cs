﻿// <auto-generated />
using System;
using MR_power.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MR_power.Migrations
{
    [DbContext(typeof(MrPower1Context))]
    partial class MrPower1ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MR_power.Data.Bill", b =>
                {
                    b.Property<int>("BillId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("BillID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BillId"));

                    b.Property<DateTime>("BillDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("BillNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CustomerId")
                        .HasColumnType("int")
                        .HasColumnName("CustomerID");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasDefaultValue("Unpaid");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    b.Property<string>("VIN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BillId")
                        .HasName("PK__Bill__11F2FC4A21BC2A77");

                    b.HasIndex("CustomerId");

                    b.HasIndex("UserId");

                    b.ToTable("Bill", (string)null);
                });

            modelBuilder.Entity("MR_power.Data.BillItem", b =>
                {
                    b.Property<int>("BillItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("BillItemID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BillItemId"));

                    b.Property<int>("BillId")
                        .HasColumnType("int")
                        .HasColumnName("BillID");

                    b.Property<decimal>("LineTotal")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("Quantity")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<int>("StookId")
                        .HasColumnType("int")
                        .HasColumnName("StookID");

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("BillItemId")
                        .HasName("PK__BillItem__47605AD55B10E016");

                    b.HasIndex("BillId");

                    b.HasIndex("StookId");

                    b.ToTable("BillItems");
                });

            modelBuilder.Entity("MR_power.Data.Customer", b =>
                {
                    b.Property<int>("CustomerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CustomerID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CustomerId"));

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("CustomerId")
                        .HasName("PK__Customer__A4AE64B85F885C63");

                    b.ToTable("Customer", (string)null);
                });

            modelBuilder.Entity("MR_power.Data.Stook", b =>
                {
                    b.Property<int>("StookId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("StookID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StookId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<decimal>("SellPrice")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(10, 2)")
                        .HasDefaultValue(0m);

                    b.Property<decimal>("UnitPrice")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("StookId")
                        .HasName("PK__Stook__00E41713DFD62E21");

                    b.ToTable("Stook", (string)null);
                });

            modelBuilder.Entity("MR_power.Data.UserAccount", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("UserID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime?>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("varchar(50)");

                    b.HasKey("UserId")
                        .HasName("PK__UserAcco__1788CCACA2A41A21");

                    b.ToTable("UserAccount", (string)null);
                });

            modelBuilder.Entity("MR_power.Data.Bill", b =>
                {
                    b.HasOne("MR_power.Data.Customer", "Customer")
                        .WithMany("Bills")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_Bill_Customer");

                    b.HasOne("MR_power.Data.UserAccount", "User")
                        .WithMany("Bills")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Bill_User");

                    b.Navigation("Customer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MR_power.Data.BillItem", b =>
                {
                    b.HasOne("MR_power.Data.Bill", "Bill")
                        .WithMany("BillItems")
                        .HasForeignKey("BillId")
                        .IsRequired()
                        .HasConstraintName("FK_BillItems_Bill");

                    b.HasOne("MR_power.Data.Stook", "Stook")
                        .WithMany("BillItems")
                        .HasForeignKey("StookId")
                        .IsRequired()
                        .HasConstraintName("FK_BillItems_Stook");

                    b.Navigation("Bill");

                    b.Navigation("Stook");
                });

            modelBuilder.Entity("MR_power.Data.Bill", b =>
                {
                    b.Navigation("BillItems");
                });

            modelBuilder.Entity("MR_power.Data.Customer", b =>
                {
                    b.Navigation("Bills");
                });

            modelBuilder.Entity("MR_power.Data.Stook", b =>
                {
                    b.Navigation("BillItems");
                });

            modelBuilder.Entity("MR_power.Data.UserAccount", b =>
                {
                    b.Navigation("Bills");
                });
#pragma warning restore 612, 618
        }
    }
}
