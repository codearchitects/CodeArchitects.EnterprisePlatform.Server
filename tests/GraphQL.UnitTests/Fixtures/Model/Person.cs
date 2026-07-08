namespace CodeArchitects.Platform.GraphQL.Fixtures.Model;

internal class Person
{
  public required Guid Id { get; set; }
  public required string GivenName { get; set; }
  public required string FamilyName { get; set; }
}
