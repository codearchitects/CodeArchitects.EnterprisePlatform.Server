using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Typology
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public List<Category>? Categories { get; set; } // MTM

  private static readonly Faker<Typology> s_faker = new Faker<Typology>()
    .RuleFor(typology => typology.Id, Guid.NewGuid)
    .RuleFor(typology => typology.Name, faker => faker.Lorem.Word());

  public static Typology One() => s_faker.Generate();
}
