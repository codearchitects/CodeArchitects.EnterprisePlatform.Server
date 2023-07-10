namespace CodeArchitects.Platform.Common.Utils;

public class SynchronizerTests
{
  [Fact]
  public void Lock_ShouldSynchronizeWithoutLockingTheKey()
  {
    // Arrange
    LocalLockUser user = new();
    object key = new();

    ParameterizedThreadStart threadStart = delegate (object? data)
    {
      ((ThreadData)data!).Deconstruct(out LocalLockUser user, out object key);

      user.SynchronizedOperationAsync(key);
    };

    Thread thread1 = new Thread(threadStart);
    Thread thread2 = new Thread(threadStart);

    // Act
    thread1.Start(new ThreadData(user, key));
    thread2.Start(new ThreadData(user, key));

    bool isLocked = !Monitor.TryEnter(key);

    thread1.Join();
    thread2.Join();

    // Assert
    user.RaceConditionHappened.Should().BeFalse();
    user.AccessCount.Should().Be(2);
    isLocked.Should().BeFalse();
  }

  private record ThreadData(LocalLockUser User, object Key);

  private class LocalLockUser
  {
    private readonly Synchronizer _synchronizer;
    private bool _busy;

    public LocalLockUser()
    {
      _synchronizer = new();
    }

    public int AccessCount { get; private set; }
    public bool RaceConditionHappened { get; private set; }

    public void SynchronizedOperationAsync(object key)
    {
      using (_synchronizer.Lock(key))
      {
        StartOperation();
        Thread.Sleep(2000);
        EndOperation();
      }
    }

    private void StartOperation()
    {
      RaceConditionHappened |= _busy;

      _busy = true;
    }

    private void EndOperation()
    {
      AccessCount++;

      _busy = false;
    }
  }
}
