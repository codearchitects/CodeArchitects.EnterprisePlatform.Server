using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class User
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public List<Cart>? Carts { get; set; } // OTM composition, on principal
  public List<UserClaim>? Claims { get; set; } // OTM aggregation, on principal
  public Address? Address { get; set; } // OTO aggregation, on principal

  private static readonly Faker<User> s_faker = new Faker<User>()
    .RuleFor(user => user.Id, Guid.NewGuid);

  public static User One() => s_faker.Generate();
}
