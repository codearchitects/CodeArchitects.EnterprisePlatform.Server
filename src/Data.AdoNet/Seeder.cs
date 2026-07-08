using CodeArchitects.Platform.Data.AdoNet.Command;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet;

internal class Seeder<TDbConnection, TDbCommand> : ISeeder
  where TDbConnection : DbConnection
  where TDbCommand : DbCommand
{
  private readonly IStateManager<TDbConnection> _stateManager;
  private readonly ICommandBuilder<TDbCommand> _commandBuilder;
  private readonly IDataModel _model;
  private readonly List<SeedEntry> _entries;

  public Seeder(IStateManager<TDbConnection> stateManager, ICommandBuilder<TDbCommand> commandBuilder, IDataModel model)
  {
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

    _stateManager.Save();
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
      SeedEntry rootEntry = new(entity, entityModel, default);
      if (!_entries.Contains(rootEntry, EntryEqualityComparer.Instance))
      {
        _entries.Add(rootEntry);
      }

      Graph.Visit(entity, entityModel, null as object, (node, model, navigation, _) =>
      {
        if (navigation == default)
          return true;

        if (navigation.NavigationModel.From == navigation.NavigationModel.To)
          throw new InvalidOperationException("Seeding entities with self reference is not supported.");

        int index = _entries.FindIndex(entry => entry.Entity == node && entry.Model == model && entry.NavigationContext == default);
        if (index != -1)
        {
          _entries.RemoveAt(index);
        }

        SeedEntry entry = new(node, model, navigation);
        if (_entries.Contains(entry, EntryEqualityComparer.Instance))
          return false;

        if (navigation.NavigationModel.IsOnDependent)
        {
          _entries.Insert(0, entry);
        }
        else
        {
          _entries.Add(entry);
        }

        if (navigation.NavigationModel is ISkipNavigationModel skipNavigation)
        {
          object junction = skipNavigation.CreateJunction(navigation.Parent, node);
          _entries.Add(new (junction, skipNavigation.JunctionEntity, navigation));
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

  private sealed class EntryEqualityComparer : IEqualityComparer<SeedEntry>
  {
    public static readonly EntryEqualityComparer Instance = new();

    public bool Equals(SeedEntry x, SeedEntry y)
    {
      return (x.Entity, x.Model) == (y.Entity, y.Model);
    }

    public int GetHashCode(SeedEntry obj)
    {
      return HashCode.Combine(obj.Entity, obj.Model);
    }
  }

  private readonly record struct SeedEntry(object Entity, IEntityModel Model, NavigationContext NavigationContext);
}
