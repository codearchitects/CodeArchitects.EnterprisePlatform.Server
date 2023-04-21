using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

internal class StateManager : Data.StateManager, IStateManager
{
  private readonly IMongoClient _client;
  private readonly List<Func<CancellationToken, Task>> _executions;

  public StateManager(IMongoClient client)
  {
    _client = client;
    _executions = new(2);
  }

  public void AddExecution(Func<CancellationToken, Task> execution)
  {
    _executions.Add(execution);
  }

  protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
  {
    IClientSessionHandle session = await _client.StartSessionAsync(cancellationToken: cancellationToken);

    if (_executions.Count > 1)
    {
      session.StartTransaction();
    }

    try
    {
      foreach (var execution in _executions)
      {
        await execution(cancellationToken);
      }
      if (session.IsInTransaction)
      {
        await session.CommitTransactionAsync();
      }
    }
    catch when (session.IsInTransaction)
    {
      await session.AbortTransactionAsync(cancellationToken);
      throw;
    }
    finally
    {
      session.Dispose();
      _executions.Clear();
    }
  }
}
