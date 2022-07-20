using System.Collections.Concurrent;

namespace Subscriber;

public class MessageAwaiter
{
  private readonly ConcurrentDictionary<Guid, TaskCompletionSource> _sources;

  public MessageAwaiter()
  {
    _sources = new();
  }

  public Task GetTask(Guid id)
  {
    TaskCompletionSource tcs = new();
    _sources[id] = tcs;
    return tcs.Task;
  }

  public void Complete(Guid id)
  {
    _sources[id].SetResult();
  }
}
