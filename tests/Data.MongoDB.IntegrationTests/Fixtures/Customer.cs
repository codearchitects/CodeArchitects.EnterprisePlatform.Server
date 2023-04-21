using Bogus;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

[Table("Customer")]
public class Customer
{
  public Guid Id { get; set; }
  public string? Name { get; set; }

  private static readonly Faker<Customer> s_faker = new Faker<Customer>()
    .RuleFor(customer => customer.Id, Guid.NewGuid)
    .RuleFor(customer => customer.Name, faker => faker.Name.FullName());

  public static Customer One() => s_faker.Generate();
}
