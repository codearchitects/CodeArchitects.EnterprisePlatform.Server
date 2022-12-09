using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class ModelConfiguration : INavigationIdGenerator
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
  private readonly HashSet<NavigationModelBuilder> _navigationBuilders;
  private bool _configured;
  private int _navigationId;

  public ModelConfiguration()
  {
    _entityBuilders = new();
    _navigationBuilders = new();
  }

  protected abstract void Configure();

  internal DataModel CreateDataModel()
  {
    if (!_configured)
    {
      Configure();
      _configured = true;
    }

    List<EntityModel> entities = new();

    DataModel dataModel = new();

    foreach (EntityModelBuilder entityBuilder in _entityBuilders.Values)
    {
      EntityModel entity = entityBuilder.Build();
      entities.Add(entity);
      dataModel.AddEntity(entity);
    }

    foreach (NavigationModelBuilder navigationBuilder in _navigationBuilders)
    {
      (NavigationModel directNavigation, NavigationModel inverseNavigation) = navigationBuilder.Build(dataModel);

      EntityModelBuilder fromEntityBuilder = _entityBuilders[directNavigation.From.Name];
      EntityModelBuilder toEntityBuilder = _entityBuilders[directNavigation.To.Name];

      fromEntityBuilder.AddNavigation(directNavigation, Enumerable.Empty<Name>());
      toEntityBuilder.AddNavigation(inverseNavigation, navigationBuilder.ForeignKeyNames);
    }

    foreach (EntityModelBuilder entityBuilder in _entityBuilders.Values)
    {
      entityBuilder.AddColumns();
    }

    return dataModel;
  }

  protected void Entity<TEntity>(Action<IEntityModelBuilder<TEntity>>? buildAction = null)
    where TEntity : class
  {
    EntityCore(
      typeof(TEntity).Name,
      buildAction);
  }

  protected void Entity<TEntity>(string entityName, Action<IEntityModelBuilder<TEntity>>? buildAction = null)
    where TEntity : class
  {
    EntityCore(
      entityName ?? throw new ArgumentNullException(nameof(entityName)),
      buildAction);
  }

  protected void Aggregation<TFrom, TTo>(Action<IAggregationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      typeof(TFrom).Name,
      typeof(TTo).Name,
      AssociationKind.Aggregation,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void Aggregation<TFrom, TTo>(string fromName, string toName, Action<IAggregationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      fromName,
      toName,
      AssociationKind.Aggregation,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void Composition<TFrom, TTo>(Action<ICompositionBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      typeof(TFrom).Name,
      typeof(TTo).Name,
      AssociationKind.Composition,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void Composition<TFrom, TTo>(string fromName, string toName, Action<ICompositionBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    Association(
      fromName,
      toName,
      AssociationKind.Composition,
      buildAction ?? throw new ArgumentNullException(nameof(buildAction)));
  }

  protected void EntityCore<TEntity>(string entityName, Action<IEntityModelBuilder<TEntity>>? buildAction = null)
    where TEntity : class
  {
    if (_entityBuilders.TryGetValue(entityName, out EntityModelBuilder? untypedBuilder))
    {
      if (untypedBuilder is not EntityModelBuilder<TEntity> typedBuilder)
        throw new ModelConfigurationException($"Duplicate entity name '{entityName}' for entities of type '{typeof(TEntity).Name}' and '{untypedBuilder.EntityType.Name}'.");

      buildAction?.Invoke(typedBuilder);
      return;
    }

    EntityModelBuilder<TEntity> builder = new(entityName);
    _entityBuilders.Add(entityName, builder);
    buildAction?.Invoke(builder);
  }

  private void Association<TFrom, TTo>(string fromName, string toName, AssociationKind kind, Action<AssociationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    AssociationBuilder<TFrom, TTo> builder = new(this, kind, fromName, toName);
    buildAction(builder);

    NavigationModelBuilder navigationBuilder = builder.NavigationBuilder;

    if (_navigationBuilders.Contains(navigationBuilder))
      throw new ModelConfigurationException($"Duplicate association '{fromName}' -> '{toName}'.");

    _navigationBuilders.Add(navigationBuilder);
  }

  protected internal static ColumnName Column(string columnName)
  {
    return new(columnName);
  }

  internal static bool IsSupportedColumnType(Type type)
  {
    return s_supportedColumnTypes.Contains(type);
  }

  int INavigationIdGenerator.GetNextId()
  {
    return ++_navigationId;
  }
}
