using Bogus;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures.Model;

public class SoftDeleteEntity
{
  public Guid Id { get; set; }
  public string? Name { get; set; }

  public const string SoftDeletePropertyName = "IsDeleted";

  private static readonly Faker<SoftDeleteEntity> s_faker = new Faker<SoftDeleteEntity>()
    .RuleFor(entity => entity.Id, Guid.NewGuid)
    .RuleFor(entity => entity.Name, faker => faker.Lorem.Word());

  public static SoftDeleteEntity One() => s_faker.Generate();
}
