using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.Fixtures.Model;

namespace CodeArchitects.Platform.Data.Fixtures;

internal class TestModelConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Entity<Cart>();

    Entity<CartItem>(entity => entity
      .WithKey(item => new { item.Index, item.CartId }));

    Entity<Product>();

    Entity<ShippingAddress>();

    Entity<Category>();

    Entity<Typology>();

    Entity<User>();

    Entity<Address>();

    Entity<UserClaim>();

    Entity<Person>();

    Aggregation<Cart, CartItem>(aggregation => aggregation
      .OneToMany()
      .Navigation(cart => cart.Items)
      .InverseNavigation(item => item.Cart)
      .UsingForeignKey(item => item.CartId));

    Composition<User, Cart>(composition => composition
      .OneToMany()
      .Navigation(user => user.Carts)
      .InverseNavigation(cart => cart.User)
      .UsingForeignKey(Column("UserId")));

    Composition<ShippingAddress, CartItem>(composition => composition
      .OneToMany()
      .InverseNavigation(item => item.ShippingAddress)
      .UsingForeignKey(Column("ShippingAddressId")));

    Composition<CartItem, Product>(composition => composition
      .ManyToMany()
      .Navigation(item => item.Products)
      .UsingJoinTable("CartItemProduct")
      .UsingJoinColumnNames("CartItemCartId", "CartItemIndex", "ProductId"));

    Composition<Category, Product>(composition => composition
      .OneToMany()
      .InverseNavigation(product => product.Category)
      .UsingForeignKey(Column("CategoryId")));

    Composition<Typology, Product>(composition => composition
      .OneToMany()
      .InverseNavigation(product => product.Typology)
      .UsingForeignKey(Column("TypologyId")));

    Composition<Category, Typology>(composition => composition
      .ManyToMany()
      .Navigation(category => category.Typologies)
      .InverseNavigation(typology => typology.Categories)
      .UsingJoinTable("CategoryTypology")
      .UsingJoinColumnNames("CategoryId", "TypologyId"));

    Aggregation<User, Address>(aggregation => aggregation
      .OneToOne()
      .Navigation(user => user.Address)
      .InverseNavigation(address => address.User)
      .UsingForeignKey(Column("UserId")));

    Aggregation<User, UserClaim>(aggregation => aggregation
      .OneToMany()
      .Navigation(user => user.Claims)
      .UsingForeignKey(Column("UserId")));

    Composition<Person, Person>(composition => composition
      .OneToOne()
      .Navigation(person => person.Partner)
      .InverseNavigation(person => person.Partner)
      .UsingForeignKey(Column("PartnerId")));
  }
}
