using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class TenantEntity
{
  public Guid Id { get; set; }
  public string? Name { get; set; }

  public const string TenantIdPropertyName = "TenantId";

  private static readonly Faker<TenantEntity> s_faker = new Faker<TenantEntity>()
    .RuleFor(entity => entity.Id, Guid.NewGuid)
    .RuleFor(entity => entity.Name, faker => faker.Lorem.Word());

  public static TenantEntity One() => s_faker.Generate();
}
