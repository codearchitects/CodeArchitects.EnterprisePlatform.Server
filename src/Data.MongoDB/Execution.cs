using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// A pending write, recorded in both its synchronous and asynchronous form.
/// </summary>
/// <remarks>
/// Keeping the two forms side by side lets the synchronous commit path call the driver's
/// synchronous APIs instead of blocking on a <see cref="Task"/>. Blocking would hold a thread
/// while a transaction is open, which is the worst possible place for a deadlock.
/// </remarks>
internal readonly struct Execution
{
  public Execution(
    Action<IClientSessionHandle> sync,
    Func<IClientSessionHandle, CancellationToken, Task> async)
  {
    Sync = sync;
    Async = async;
  }

  public Action<IClientSessionHandle> Sync { get; }

  public Func<IClientSessionHandle, CancellationToken, Task> Async { get; }
}
