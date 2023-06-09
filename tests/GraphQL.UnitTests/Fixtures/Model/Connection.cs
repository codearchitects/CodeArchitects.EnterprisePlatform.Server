namespace CodeArchitects.Platform.GraphQL.Fixtures.Model;

internal class Connection<T>
  where T : class
{
  public PageInfo? PageInfo { get; set; }
  public Edge<T>[]? Edges { get; set; }
  public T[]? Nodes { get; set; }
}

internal class PageInfo
{
  public bool HasPreviousPage { get; set; }
  public bool HasNextPage { get; set; }
  public string? StartCursor { get; set; }
  public string? EndCursor { get; set; }
}

internal class Edge<T>
  where T : class
{
  public string? Cursor { get; set; }
  public T? Node { get; set; }
}
