using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Cart
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public List<CartItem>? Items { get; set; } // OTM intra-aggregate, on principal
  public Customer? Customer { get; set; } // OTM inter-aggregate, on dependent

  private static readonly Faker<Cart> s_faker = new Faker<Cart>()
    .RuleFor(cart => cart.Id, Guid.NewGuid)
    .RuleFor(cart => cart.Name, faker => faker.Lorem.Word());

  public static Cart One() => s_faker.Generate();

  public static List<Cart> Many(int count) => s_faker.Generate(count);
}
