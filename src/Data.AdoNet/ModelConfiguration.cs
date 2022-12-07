using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class ModelConfiguration
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

  private readonly Dictionary<string, EntityModelBuilder> _entityBuilders;
  private readonly HashSet<Association> _associations;
  private bool _configured;

  public ModelConfiguration()
  {
    _entityBuilders = new();
    _associations = new(AssociationEqualityComparer.Instance);
  }

  protected abstract void Configure();

  internal DataModel CreateDataModel()
  {
    if (!_configured)
    {
      Configure();
      _configured = true;
    }

    foreach (Association association in _associations)
    {
      if (!_entityBuilders.Values.Any(builder => builder.EntityType == association.From))
        throw new ModelConfigurationException($"An association '{association.From.Name}' -> '{association.To.Name}' was defined, but '{association.From.Name}' is not an entity type.");
    
      if (!_entityBuilders.Values.Any(builder => builder.EntityType == association.To))
        throw new ModelConfigurationException($"An association '{association.From.Name}' -> '{association.To.Name}' was defined, but '{association.To.Name}' is not an entity type.");
    }

    return DataModel.Create(_entityBuilders.Values, _associations);
  }

  protected void Entity<TEntity>(Action<IEntityModelBuilder<TEntity>> buildAction)
    where TEntity : class
  {
    EntityCore(
      typeof(TEntity).Name,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void Entity<TEntity>(string entityName, Action<IEntityModelBuilder<TEntity>> buildAction)
    where TEntity : class
  {
    EntityCore(
      entityName ?? throw new ArgumentNullException(nameof(entityName)),
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void Aggregation<TFrom, TTo>(Action<IAggregationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      AssociationKind.Aggregation,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void Composition<TFrom, TTo>(Action<ICompositionBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      AssociationKind.Composition,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void EntityCore<TEntity>(string entityName, Action<IEntityModelBuilder<TEntity>> buildAction)
    where TEntity : class
  {
    EntityModelBuilder<TEntity> builder = GetEntityBuilder<TEntity>(entityName);
    buildAction(builder);
  }

  private EntityModelBuilder<TEntity> GetEntityBuilder<TEntity>(string entityName)
    where TEntity : class
  {
    if (_entityBuilders.TryGetValue(entityName, out EntityModelBuilder? builder))
      return builder as EntityModelBuilder<TEntity>
        ?? throw new ModelConfigurationException($"Duplicate entity name '{entityName}' for entities of type '{typeof(TEntity).Name}' and '{builder.EntityType.Name}'.");

    EntityModelBuilder<TEntity> typedBuilder = new(entityName);
    _entityBuilders.Add(entityName, typedBuilder);

    return typedBuilder;
  }

  private void Association<TFrom, TTo>(AssociationKind kind, Action<AssociationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    AssociationBuilder<TFrom, TTo> builder = new();
    buildAction(builder);
    Association association = builder.Build(kind);

    if (_associations.Contains(association))
      throw new ModelConfigurationException($"Duplicate association '{typeof(TFrom).Name}' -> '{typeof(TTo).Name}'.");

    _associations.Add(association);
  }

  protected internal static ColumnName Column(string columnName)
  {
    return new(columnName);
  }

  internal static bool IsSupportedColumnType(Type type)
  {
    return s_supportedColumnTypes.Contains(type);
  }
}
