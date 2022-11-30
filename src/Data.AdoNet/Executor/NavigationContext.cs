namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal readonly struct NavigationContext
{
  public NavigationContext(int navigationId, object?[] keyValues)
  {
    NavigationId = navigationId;
    KeyValues = keyValues;
  }

  public int NavigationId { get; }
  public object?[] KeyValues { get; }
}