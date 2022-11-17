using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class UserClaim
{
  public Guid Id { get; set; }
  public string? Name { get; set; }

  private static readonly Faker<UserClaim> s_faker = new Faker<UserClaim>()
    .RuleFor(claim => claim.Id, Guid.NewGuid)
    .RuleFor(claim => claim.Name, faker => faker.Lorem.Word());

  public static UserClaim One() => s_faker.Generate();

  public static List<UserClaim> Many(int count) => s_faker.Generate(count);
}
