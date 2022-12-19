using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Associations;
using CodeArchitects.Platform.Data.Fixtures.Model;
using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.Fixtures;

public class TestDbContext : DbContext
{
  protected TestDbContext() { }

  public TestDbContext(DbContextOptions options)
    : base(options)
  {
  }

  public override int SaveChanges(bool acceptAllChangesOnSuccess)
  {
    try
    {
      return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    finally
    {
      ChangeTracker.Clear();
    }
  }

  public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
  {
    try
    {
      return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    finally
    {
      ChangeTracker.Clear();
    }
  }

  protected sealed override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<Cart>(entity =>
    {
      entity
        .HasMany(cart => cart.Items)
        .WithOne(item => item.Cart!)
        .HasForeignKey(item => item.CartId)
        .AsAggregation();

      entity
        .HasOne(cart => cart.Customer!)
        .WithMany(customer => customer.Carts!)
        .AsComposition();
    });

    modelBuilder.Entity<CartItem>(entity =>
    {
      entity
        .HasKey(item => new { item.Index, item.CartId });

      entity
        .HasOne(item => item.ShippingAddress!)
        .WithMany()
        .IsRequired(false)
        .AsComposition();

      entity
        .HasMany(item => item.Products)
        .WithMany(product => product.CartItems)
        .UsingEntity<Dictionary<string, object>>(
          "CartItemProduct",
          join => join
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey("ProductId"),
          join => join
            .HasOne<CartItem>()
            .WithMany()
            .HasForeignKey("CartItemIndex", "CartItemCartId"));
    });

    modelBuilder.Entity<Product>(entity =>
    {
      entity
        .HasOne(product => product.Category!)
        .WithMany()
        .AsComposition();

      entity
        .HasOne(product => product.Typology!)
        .WithMany()
        .AsComposition();
    });

    modelBuilder.Entity<Category>(entity =>
    {
      entity
        .HasMany(category => category.Typologies)
        .WithMany(typology => typology.Categories)
        .UsingEntity<Dictionary<string, object>>(
          "CategoryTypology",
          join => join
            .HasOne<Typology>()
            .WithMany()
            .HasForeignKey("TypologyId")
            .OnDelete(DeleteBehavior.Cascade),
          join => join
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Cascade));
    });

    modelBuilder.Entity<Customer>(entity =>
    {
      entity
        .HasMany(customer => customer.Claims)
        .WithOne()
        .AsAggregation();
    });

    modelBuilder.Entity<Address>(entity =>
    {
      entity
        .HasOne(address => address.Customer!)
        .WithOne(customer => customer.Address!)
        .HasForeignKey<Address>("CustomerId")
        .AsAggregation();
    });

    modelBuilder.Entity<Person>(entity =>
    {
      entity
        .HasOne(person => person.Partner!)
        .WithOne()
        .AsComposition();
    });

    modelBuilder.Entity<SerialEntity>();
  }
}

public class SqlServerDbContext : TestDbContext // For SqlServer migrations
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder
      .UseSqlServer("-")
      .UseData();
  }
}

public class PostgresDbContext : TestDbContext // For Postgres migrations
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder
      .UseNpgsql("-")
      .UseData();
  }
}
