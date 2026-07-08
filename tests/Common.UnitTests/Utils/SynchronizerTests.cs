namespace CodeArchitects.Platform.Common.Utils;

public class SynchronizerTests
{
  [Fact]
  public void Sync_ShouldSynchronizeWithoutLockingTheKey_UsingThreads()
  {
    // Arrange
    const int threadCount = 5;
    LocalLockUser user = new();
    object key = new();
    OperationData data = new(user, key);

    ParameterizedThreadStart threadStart = delegate(object? data)
    {
      ((OperationData)data!).Deconstruct(out LocalLockUser user, out object key);

      user.SynchronizedOperation(key);
    };

    List<Thread> threads = Enumerable.Range(0, threadCount).Select(_ => new Thread(threadStart)).ToList();

    // Act
    threads.ForEach(thread => thread.Start(data));
    bool isLocked = !Monitor.TryEnter(key);
    threads.ForEach(thread => thread.Join());

    // Assert
    user.RaceConditionHappened.Should().BeFalse();
    user.AccessCount.Should().Be(threadCount);
    isLocked.Should().BeFalse();
  }

  [Fact]
  public async Task Sync_ShouldSynchronizeWithoutLockingTheKey_UsingTasks()
  {
    // Arrange
    const int taskCount = 5;
    LocalLockUser user = new();
    object key = new();

    Action taskAction = delegate()
    {
      user.SynchronizedOperation(key);
    };

    List<Task> tasks = Enumerable.Range(0, taskCount).Select(_ => Task.Run(taskAction)).ToList();

    // Act
    bool isLocked = !Monitor.TryEnter(key);
    await Task.WhenAll(tasks);

    // Assert
    user.RaceConditionHappened.Should().BeFalse();
    user.AccessCount.Should().Be(taskCount);
    isLocked.Should().BeFalse();
  }

  private record OperationData(LocalLockUser User, object Key);

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

    public void SynchronizedOperation(object key)
    {
      using (_synchronizer.Sync(key))
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
