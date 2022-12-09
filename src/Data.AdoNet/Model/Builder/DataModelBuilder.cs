using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class DataModelBuilder : INavigationIdGenerator
{
  private readonly Dictionary<string, EntityModelBuilder> _entityBuilders;
  private readonly HashSet<NavigationModelBuilder> _navigationBuilders;
  private int _navigationId;

  public DataModelBuilder()
  {
    _entityBuilders = new();
    _navigationBuilders = new();
  }

  public DataModel Build()
  {
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

  public EntityModelBuilder<TEntity> GetEntityBuilder<TEntity>(string entityName)
    where TEntity : class
  {
    if (_entityBuilders.TryGetValue(entityName, out EntityModelBuilder? untypedBuilder))
      return untypedBuilder as EntityModelBuilder<TEntity>
        ?? throw new ModelConfigurationException($"Duplicate entity name '{entityName}' for entities of type '{typeof(TEntity).Name}' and '{untypedBuilder.EntityType.Name}'.");

    EntityModelBuilder<TEntity> builder = new(entityName);
    _entityBuilders.Add(entityName, builder);

    return builder;
  }

  public void AddNavigationBuilder(string fromEntityName, string toEntityName, NavigationModelBuilder navigationBuilder)
  {
    if (_navigationBuilders.Contains(navigationBuilder))
      throw new ModelConfigurationException($"Duplicate association '{fromEntityName}' -> '{toEntityName}'.");

    _navigationBuilders.Add(navigationBuilder);
  }

  public int GetNextId()
  {
    return ++_navigationId;
  }
}
