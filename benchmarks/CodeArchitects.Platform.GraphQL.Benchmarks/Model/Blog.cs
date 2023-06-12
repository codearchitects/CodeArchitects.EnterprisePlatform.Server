namespace CodeArchitects.Platform.GraphQL.Fixtures.Model;

internal class Blog
{
  public required Guid Id { get; set; }
  public required string Name { get; set; }
  public Person? Owner { get; set; }
  public Connection<Post>? Posts { get; set; }
}
