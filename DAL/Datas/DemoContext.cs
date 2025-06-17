using System;
using System.Collections.Generic;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Datas;

public partial class DemoContext : DbContext
{
    public DemoContext()
    {
    }

    public DemoContext(DbContextOptions<DemoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<AccountStatus> AccountStatuses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentStatus> CommentStatuses { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductItem> ProductItems { get; set; }

    public virtual DbSet<ProductItemStatus> ProductItemStatuses { get; set; }

    public virtual DbSet<ProductStatus> ProductStatuses { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<RevenueLog> RevenueLogs { get; set; }

    public virtual DbSet<RoleName> RoleNames { get; set; }

    public virtual DbSet<Variation> Variations { get; set; }

    public virtual DbSet<VariationOption> VariationOptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3213E83F5C2A60C3");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Email, "UQ__Account__AB6E616401BDCE69").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Account__role_id__45F365D3");

            entity.HasOne(d => d.Status).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Account__status___46E78A0C");
        });

        modelBuilder.Entity<AccountStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account___3213E83FB73FC788");

            entity.ToTable("Account_Status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3213E83F1A789986");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment__3213E83F9987954D");

            entity.ToTable("Comment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__Comment__parent___693CA210");

            entity.HasOne(d => d.Product).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Comment__product__68487DD7");

            entity.HasOne(d => d.Status).WithMany(p => p.Comments)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Comment__status___6A30C649");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comment__user_id__6754599E");
        });

        modelBuilder.Entity<CommentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Comment___3213E83F9AD4D84C");

            entity.ToTable("Comment_Status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3213E83F7F539C85");

            entity.ToTable("Order");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Order__status_id__5AEE82B9");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order__user_id__59FA5E80");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_It__3213E83FCF90A84A");

            entity.ToTable("Order_Item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProductItemId).HasColumnName("product_item_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Order_Ite__order__5DCAEF64");

            entity.HasOne(d => d.ProductItem).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductItemId)
                .HasConstraintName("FK__Order_Ite__produ__5EBF139D");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_St__3213E83F702863CE");

            entity.ToTable("Order_Status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3213E83F7C1DD8B5");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Picture)
                .HasMaxLength(50)
                .HasColumnName("picture");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Product__categor__49C3F6B7");

            entity.HasOne(d => d.Status).WithMany(p => p.Products)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Product__status___4AB81AF0");
        });

        modelBuilder.Entity<ProductItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product___3213E83F1F99D8FC");

            entity.ToTable("Product_Item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.ImportPrice).HasColumnName("import_price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.SellingPrice).HasColumnName("selling_price");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Product_I__produ__4D94879B");

            entity.HasOne(d => d.Status).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Product_I__statu__4E88ABD4");

            entity.HasMany(d => d.VariationOptions).WithMany(p => p.ProductItems)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductConfiguration",
                    r => r.HasOne<VariationOption>().WithMany()
                        .HasForeignKey("VariationOptionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Product_C__varia__571DF1D5"),
                    l => l.HasOne<ProductItem>().WithMany()
                        .HasForeignKey("ProductItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Product_C__produ__5629CD9C"),
                    j =>
                    {
                        j.HasKey("ProductItemId", "VariationOptionId").HasName("PK__Product___B8C15F9DBD00281F");
                        j.ToTable("Product_Configuration");
                        j.IndexerProperty<int>("ProductItemId").HasColumnName("product_item_id");
                        j.IndexerProperty<int>("VariationOptionId").HasColumnName("variation_option_id");
                    });
        });

        modelBuilder.Entity<ProductItemStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product___3213E83FE38571D7");

            entity.ToTable("Product_Item_Status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product___3213E83F508413DF");

            entity.ToTable("Product_Status");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rating__3213E83F8395ABC3");

            entity.ToTable("Rating");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UC_UserProduct").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.RatingValue).HasColumnName("rating_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Rating__product___6477ECF3");

            entity.HasOne(d => d.User).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Rating__user_id__6383C8BA");
        });

        modelBuilder.Entity<RevenueLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Revenue___3213E83F15A2C4FA");

            entity.ToTable("Revenue_Log");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.TotalCost).HasColumnName("total_cost");
            entity.Property(e => e.TotalProfit).HasColumnName("total_profit");
            entity.Property(e => e.TotalRevenue).HasColumnName("total_revenue");

            entity.HasOne(d => d.Order).WithMany(p => p.RevenueLogs)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Revenue_L__order__6D0D32F4");
        });

        modelBuilder.Entity<RoleName>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role_Nam__3213E83F9D12FC27");

            entity.ToTable("Role_Name");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Variation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Variatio__3213E83F770C757A");

            entity.ToTable("Variation");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<VariationOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Variatio__3213E83FF916763D");

            entity.ToTable("Variation_Option");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .HasColumnName("value");
            entity.Property(e => e.VariationId).HasColumnName("variation_id");

            entity.HasOne(d => d.Variation).WithMany(p => p.VariationOptions)
                .HasForeignKey(d => d.VariationId)
                .HasConstraintName("FK__Variation__varia__534D60F1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
