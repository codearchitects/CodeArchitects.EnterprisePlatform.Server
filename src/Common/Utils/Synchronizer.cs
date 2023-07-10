using System.Collections.Concurrent;

namespace CodeArchitects.Platform.Common.Utils;

/// <summary>
/// Provides a mechanism which synchronizes a block of code without acquiring an actual lock on an object.
/// </summary>
/// <remarks>
/// If injected as a service, its lifetime must be TRANSIENT.
/// </remarks>
internal class Synchronizer
{
  private readonly ConcurrentDictionary<object, SyncToken> _tokens;
  private readonly Func<object, SyncToken> _createToken;

  public Synchronizer()
  {
    _tokens = new();

    // Not sure if the compiler is smart enough to avoid allocating a new delegate each time, because 'this' is captured.
    // As of today, SharpLab suggests it is not.
    _createToken = key => new SyncToken(this, key);
  }

  public IDisposable Lock<T>(T key)
    where T : class
  {
    SyncToken token = _tokens.GetOrAdd(key, _createToken);
    token.Enter();
    return token;
  }

  private void Release(object key)
  {
    _tokens.TryRemove(key, out _);
  }

  private sealed class SyncToken : IDisposable
  {
    private readonly Synchronizer _synchronizer;
    private readonly object _key;
    private bool _lockTaken;

    public SyncToken(Synchronizer synchronizer, object key)
    {
      _synchronizer = synchronizer;
      _key = key;
    }

    public void Enter()
    {
      bool lockTaken = false;
      Monitor.Enter(this, ref lockTaken);

      _lockTaken = lockTaken;
    }

    void IDisposable.Dispose()
    {
      if (_lockTaken)
      {
        Monitor.Exit(this);
      }

      _synchronizer.Release(_key);
      _lockTaken = false;
    }
  }
}
