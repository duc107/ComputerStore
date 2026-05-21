using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ComputerStore.Models;

public partial class ComputerStoreDbContext : DbContext
{
    public ComputerStoreDbContext()
    {
    }

    public ComputerStoreDbContext(DbContextOptions<ComputerStoreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appuser> Appusers { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Chatmessage> Chatmessages { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Warrantyticket> Warrantytickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Appuser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("appusers");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasColumnType("text");
            entity.Property(e => e.Role).HasMaxLength(20);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Chatmessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PRIMARY");

            entity.ToTable("chatmessages");

            entity.HasIndex(e => e.SenderId, "SenderId");

            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Sender).WithMany(p => p.Chatmessages)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("chatmessages_ibfk_1");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.PromotionId, "PromotionId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'");
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

            entity.HasOne(d => d.Promotion).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PromotionId)
                .HasConstraintName("orders_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PRIMARY");

            entity.ToTable("orderdetails");

            entity.HasIndex(e => e.OrderId, "OrderId");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);

            entity.HasOne(d => d.Order).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("orderdetails_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("orderdetails_ibfk_2");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("products");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.ImageUrl).HasColumnType("text");
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.ProductName).HasMaxLength(200);
            entity.Property(e => e.StockQuantity).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("products_ibfk_1");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PRIMARY");

            entity.ToTable("promotions");

            entity.HasIndex(e => e.Code, "Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.DiscountPercent).HasPrecision(5, 2);
            entity.Property(e => e.ExpiryDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");

            entity.ToTable("reviews");

            entity.HasIndex(e => e.ProductId, "ProductId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("reviews_ibfk_1");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("reviews_ibfk_2");
        });

        modelBuilder.Entity<Warrantyticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PRIMARY");

            entity.ToTable("warrantytickets");

            entity.HasIndex(e => e.CustomerId, "CustomerId");

            entity.Property(e => e.IssueDescription).HasColumnType("text");
            entity.Property(e => e.ReceivedDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ReturnDate).HasColumnType("datetime");
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Received'");

            entity.HasOne(d => d.Customer).WithMany(p => p.Warrantytickets)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("warrantytickets_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
