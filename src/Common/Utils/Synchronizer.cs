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

  /// <summary>
  /// Synchronizes the block of code inside the scope of the disposable using an equivalence key.
  /// </summary>
  /// <typeparam name="T">The type of the key.</typeparam>
  /// <param name="key">The equivalence key used for synchronization.</param>
  /// <returns>A synchronization token that signals the synchronized block of code as available when disposed.</returns>
  /// <remarks>Do NOT use the await keyword inside the synchronized scope.</remarks>
  public IDisposable Sync<T>(T key)
    where T : class
  {
    SyncToken token = _tokens.GetOrAdd(key, _createToken);
    token.Sync();
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
    private int _enteringCount;

    public SyncToken(Synchronizer synchronizer, object key)
    {
      _synchronizer = synchronizer;
      _key = key;
    }

    public void Sync()
    {
      bool lockTaken = false;
      Interlocked.Increment(ref _enteringCount);
      Monitor.Enter(this, ref lockTaken);

      if (_lockTaken)
        throw new InvalidOperationException("An async block of code was synchronized using a Synchronizer instance.");

      _lockTaken = lockTaken;
    }

    void IDisposable.Dispose()
    {
      Interlocked.Decrement(ref _enteringCount);

      if (_enteringCount == 0)
      {
        _synchronizer.Release(_key);
      }

      if (_lockTaken)
      {
        _lockTaken = false;
        Monitor.Exit(this);
      }
    }
  }
}
