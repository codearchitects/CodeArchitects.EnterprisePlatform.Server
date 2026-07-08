using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class CustomerClaim
{
  public Guid Id { get; set; }
  public string? Name { get; set; }

  private static readonly Faker<CustomerClaim> s_faker = new Faker<CustomerClaim>()
    .RuleFor(claim => claim.Id, Guid.NewGuid)
    .RuleFor(claim => claim.Name, faker => faker.Lorem.Word());

  public static CustomerClaim One() => s_faker.Generate();

  public static List<CustomerClaim> Many(int count) => s_faker.Generate(count);
}
