using Bogus;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeArchitects.Platform.Data.Fixtures.Model;

internal class SerialEntity
{
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  public string? Name { get; set; }

  private static readonly Faker<SerialEntity> s_faker = new Faker<SerialEntity>()
    .RuleFor(entity => entity.Name, faker => faker.Name.FullName());

  public static SerialEntity One() => s_faker.Generate();

  public static List<SerialEntity> Many(int count) => s_faker.Generate(count);
}
