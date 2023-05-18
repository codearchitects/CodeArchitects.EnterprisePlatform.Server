using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

internal class ConcurrencyEntity
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public int Version { get; set; }

  public ConcurrencyEntity Clone() => new()
  {
    Id = Id,
    Name = Name,
    Version = Version
  };

  private static readonly Faker<ConcurrencyEntity> s_faker = new Faker<ConcurrencyEntity>()
    .RuleFor(customer => customer.Id, Guid.NewGuid)
    .RuleFor(customer => customer.Name, faker => faker.Name.FullName());

  public static ConcurrencyEntity One() => s_faker.Generate();
}
