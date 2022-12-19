using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Customer
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public List<Cart>? Carts { get; set; } // OTM composition, on principal
  public List<CustomerClaim>? Claims { get; set; } // OTM aggregation, on principal
  public Address? Address { get; set; } // OTO aggregation, on principal

  private static readonly Faker<Customer> s_faker = new Faker<Customer>()
    .RuleFor(customer => customer.Id, Guid.NewGuid)
    .RuleFor(customer => customer.Name, faker => faker.Name.FullName());

  public static Customer One() => s_faker.Generate();
}
