namespace Subscriber.NoResponse;

public class NoResponseAwaiter
{
  private readonly Dictionary<Guid, TaskCompletionSource> _sources;

  public NoResponseAwaiter()
  {
    _sources = new();
  }

  public Task GetTask(Guid id)
  {
    TaskCompletionSource tcs = new();
    _sources.Add(id, tcs);
    return tcs.Task;
  }

  public void Complete(Guid id)
  {
    _sources[id].SetResult();
  }
}
