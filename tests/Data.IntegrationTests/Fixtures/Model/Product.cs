using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Product
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public Category? Category { get; set; } // OTM composition, on dependent
  public Typology? Typology { get; set; } // OTM composition, on dependent

  private static readonly Faker<Product> s_faker = new Faker<Product>()
    .RuleFor(product => product.Id, Guid.NewGuid)
    .RuleFor(product => product.Name, faker => faker.Lorem.Word());

  public static Product One() => s_faker.Generate();
}
