using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using CodeArchitects.Platform.Data.Features.Associations;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet;

/// <summary>
/// This class is used to configure the persistence model of the database.
/// Subclasses will be injected as a singleton service, therefore do not use scoped dependencies inside it.
/// </summary>
public abstract class ModelConfiguration // TODO: The whole model building section needs a refactoring
{
  internal static readonly MethodInfo s_hiddenColumnMethod = typeof(ModelConfiguration).GetRequiredMethod(
    name: nameof(Column),
    bindingAttr: BindingFlags.Static | BindingFlags.NonPublic,
    types: new[] { typeof(string) });

  internal static readonly IReadOnlyCollection<Type> s_supportedColumnTypes = new HashSet<Type>()
  {
    typeof(bool),
    typeof(byte),
    typeof(char),
    typeof(DateTime),
    typeof(decimal),
    typeof(double),
    typeof(float),
    typeof(Guid),
    typeof(short),
    typeof(int),
    typeof(long),
    typeof(string),
    typeof(bool?),
    typeof(byte?),
    typeof(char?),
    typeof(DateTime?),
    typeof(decimal?),
    typeof(double?),
    typeof(float?),
    typeof(Guid?),
    typeof(short?),
    typeof(int?),
    typeof(long?)
  };

  private readonly DataModelBuilder _modelBuilder;
  private DataModel? _dataModel;

  /// <summary>
  /// Creates a new instance of <see cref="ModelConfiguration"/>.
  /// </summary>
  protected ModelConfiguration()
  {
    _modelBuilder = new();
  }

  /// <summary>
  /// This method is used to configure the persistence model of the database.
  /// </summary>
  protected abstract void Configure();

  internal IDataModel CreateDataModel()
  {
    if (_dataModel is null)
    {
      Configure();
      _dataModel = _modelBuilder.Build();
    }

    return _dataModel;
  }

  /// <summary>
  /// Configures an entity type.
  /// </summary>
  /// <typeparam name="TEntity">The entity type.</typeparam>
  /// <param name="buildAction">The action that configures the entity type.</param>
  protected void Entity<TEntity>(Action<IEntityModelBuilder<TEntity>>? buildAction = null)
    where TEntity : class
  {
    EntityModelBuilder<TEntity> entityBuilder = _modelBuilder.GetEntityBuilder<TEntity>();
    buildAction?.Invoke(entityBuilder);
  }

  /// <summary>
  /// Configures a relationship between two entities belonging to the same aggregate.
  /// </summary>
  /// <typeparam name="TFrom">The type of the "from" entity of the relationship.</typeparam>
  /// <typeparam name="TTo">The type of the "to" entity of the relationship.</typeparam>
  /// <param name="buildAction">The action that configures the relationship.</param>
  protected void Aggregate<TFrom, TTo>(Action<IIntraAggregateBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      AssociationKind.IntraAggregate,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  /// <summary>
  /// Configures a relationship between two entities belonging to different aggregates.
  /// </summary>
  /// <typeparam name="TFrom">The type of the "from" entity of the relationship.</typeparam>
  /// <typeparam name="TTo">The type of the "to" entity of the relationship.</typeparam>
  /// <param name="buildAction">The action that configures the relationship.</param>
  protected void Associate<TFrom, TTo>(Action<IInterAggregateBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      AssociationKind.InterAggregate,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  /// <summary>
  /// Specifies that a string must be interpreted as the name of a column.
  /// </summary>
  /// <param name="columnName">The name of the column.</param>
  /// <returns>A <see cref="ColumnName"/> instance.</returns>
  protected internal static ColumnName Column(string columnName)
  {
    return new(columnName);
  }

  private void Association<TFrom, TTo>(AssociationKind kind, Action<AssociationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    AssociationBuilder<TFrom, TTo> builder = new(_modelBuilder, kind);
    buildAction(builder);

    _modelBuilder.AddNavigationBuilder(builder.NavigationBuilder);
  }

  internal static bool IsSupportedColumnType(Type type)
  {
    return s_supportedColumnTypes.Contains(type);
  }
}
