namespace CodeArchitects.Platform.GraphQL.Fixtures.Model;

internal class Blog
{
  public required Guid Id { get; set; }
  public string? Name { get; set; }
  public Person? Owner { get; set; }
  public Connection<Post>? Posts { get; set; }
}

internal class ScienceBlog : Blog
{
  public string? Theme { get; set; }
}