using Bogus;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

public class Person
{
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public Person? Partner { get; set; } // OTO inter-aggregate

  private static readonly Faker<Person> s_faker = new Faker<Person>()
    .RuleFor(person => person.Id, Guid.NewGuid)
    .RuleFor(person => person.Name, faker => faker.Lorem.Word());

  public static Person One() => s_faker.Generate();

  public static List<Person> Many(int count) => s_faker.Generate(count);
}
