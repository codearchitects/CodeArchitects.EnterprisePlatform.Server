namespace CodeArchitects.Platform.GraphQL.Fixtures.Model;

internal class GetBlogsResult
{
  public required Connection<Blog> Blogs { get; set; }
}

internal class GetBlogsVariables
{
  public int First { get; set; }
  public int Last { get; set; }
  public string? Before { get; set; }
  public string? After { get; set; }
}