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

  protected override void SaveCore()
  {
    using IClientSessionHandle session = _client.StartSession();

    if (_executions.Count > 1)
    {
      session.StartTransaction();
    }

    try
    {
      foreach (var execution in _executions)
      {
        execution(CancellationToken.None).GetAwaiter().GetResult();
      }
      if (session.IsInTransaction)
      {
        session.CommitTransaction();
      }
    }
    catch when (session.IsInTransaction)
    {
      session.AbortTransaction();
      throw;
    }
    finally
    {
      _executions.Clear();
    }
  }

  protected override async Task SaveCoreAsync(CancellationToken cancellationToken)
  {
    using IClientSessionHandle session = await _client.StartSessionAsync(cancellationToken: cancellationToken);

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
      _executions.Clear();
    }
  }
}
