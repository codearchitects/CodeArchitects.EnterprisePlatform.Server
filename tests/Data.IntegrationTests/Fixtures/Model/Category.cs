using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Category
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public List<Typology>? Typologies { get; set; } // MTM inter-aggregate

  private static readonly Faker<Category> s_faker = new Faker<Category>()
    .RuleFor(category => category.Id, Guid.NewGuid)
    .RuleFor(category => category.Name, faker => faker.Lorem.Word());

  public static Category One() => s_faker.Generate();
}
