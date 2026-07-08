namespace CodeArchitects.Platform.GraphQL.Fixtures.Model;

internal class GetBlogsResult
{
  public required Connection<Blog> Blogs { get; set; }
}

internal class GetBlogsVariables
{
  public int Arg1 { get; set; }
  public string? Arg2 { get; set; }
  public int[] Arg3 { get; set; } = default!;
  public int?[]? Arg4 { get; set; }
}