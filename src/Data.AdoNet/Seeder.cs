using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class Seeder<TDbConnection, TDbCommand> : ISeeder
  where TDbConnection : DbConnection
  where TDbCommand : DbCommand
{
  private readonly IExecutor<TDbCommand> _executor;
  private readonly IStateManager<TDbConnection> _stateManager;
  private readonly ICommandBuilder<TDbCommand> _commandBuilder;
  private readonly IDataModel _model;
  private readonly List<(object Entity, IEntityModel Model, NavigationContext NavigationContext)> _entries;

  public Seeder(
    IExecutor<TDbCommand> executor,
    IStateManager<TDbConnection> stateManager,
    ICommandBuilder<TDbCommand> commandBuilder,
    IDataModel model)
  {
    _executor = executor;
    _stateManager = stateManager;
    _commandBuilder = commandBuilder;
    _model = model;
    _entries = new();
  }

  public void Apply(DataSeed seed)
  {
    seed.Seed(this);

    foreach (var item in _entries)
    {
      _stateManager.AddExecution((connection, transaction, cancellationToken) =>
      {
        TDbCommand command = (TDbCommand)connection.CreateCommand();
        command.Transaction = transaction;
        _commandBuilder.BuildInsertCommand(command, item.Entity, item.Model, item.NavigationContext);
        command.ExecuteNonQuery();
        return Task.CompletedTask;
      }, true);
    }

    _stateManager.SaveAsync(CancellationToken.None).GetAwaiter().GetResult();
    _entries.Clear();
  }

  public void Seed<TEntity>(IEnumerable<TEntity> entities)
    where TEntity : class
  {
    IEntityModel entityModel = _model.GetEntity(typeof(TEntity));

    if (!IsEmpty(_stateManager.Connection, entityModel))
      return;

    foreach (TEntity entity in entities)
    {
      if (!_entries.Contains((entity, entityModel, default), EntryEqualityComparer.Instance))
      {
        _entries.Add((entity, entityModel, default));
      }

      Graph.Visit(entity, entityModel, null as object, (node, model, navigation, _) =>
      {
        if (navigation == default)
          return true;

        if (navigation.NavigationModel.From == navigation.NavigationModel.To)
          throw new InvalidOperationException("Seeding entities with self reference is not supported.");

        int index = _entries.FindIndex(entry => entry == (node, model, default));
        if (index != -1)
        {
          _entries.RemoveAt(index);
        }

        if (_entries.Contains((node, model, navigation), EntryEqualityComparer.Instance))
          return false;

        if (navigation.NavigationModel.IsOnDependent)
        {
          AddTop(_entries, (node, model, navigation));
        }
        else
        {
          AddBottom(_entries, (node, model, navigation));
        }

        if (navigation.NavigationModel is ISkipNavigationModel skipNavigation)
        {
          object junction = skipNavigation.CreateJunction(navigation.Parent, node);
          _entries.Add((junction, skipNavigation.JunctionEntity, navigation));
        }

        return true;
      });
    }
  }

  private bool IsEmpty(TDbConnection connection, IEntityModel entityModel)
  {
    try
    {
      connection.Open();

      TDbCommand command = (TDbCommand)connection.CreateCommand();
      _commandBuilder.BuildCountCommand(command, entityModel);
      int count = Convert.ToInt32(command.ExecuteScalar());

      return count == 0;
    }
    finally
    {
      connection.Close();
    }
  }

  private static void AddTop<T>(List<T> list, T element) // yuck
  {
    list.Add(element);
    for (int i = list.Count - 1; i > 0; i--)
    {
      list[i] = list[i - 1];
    }
    list[0] = element;
  }

  private static void AddBottom<T>(List<T> list, T element)
  {
    list.Add(element);
  }

  private class EntryEqualityComparer : IEqualityComparer<(object, IEntityModel, NavigationContext)>
  {
    public static readonly EntryEqualityComparer Instance = new EntryEqualityComparer();

    public bool Equals([AllowNull] (object, IEntityModel, NavigationContext) x, [AllowNull] (object, IEntityModel, NavigationContext) y)
    {
      return (x.Item1, x.Item2) == (y.Item1, y.Item2);
    }

    public int GetHashCode([DisallowNull] (object, IEntityModel, NavigationContext) obj)
    {
      return HashCode.Combine(obj.Item1, obj.Item2);
    }
  }
}
