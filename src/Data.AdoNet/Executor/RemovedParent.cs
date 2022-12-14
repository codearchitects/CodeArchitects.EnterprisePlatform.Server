namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal sealed class RemovedParent
{
  private RemovedParent() { }

  public static readonly RemovedParent Instance = new RemovedParent();

  public static bool IsRemoved(object parent) => ReferenceEquals(parent, Instance);
}
