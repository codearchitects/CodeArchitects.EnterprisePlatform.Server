using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class DataModelBuilder : INavigationIdGenerator
{
  private readonly Dictionary<Type, EntityModelBuilder> _entityBuilders;
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
      NavigationModel navigation = navigationBuilder.Build(dataModel);

      EntityModelBuilder fromEntityBuilder = _entityBuilders[navigation.From.Type];
      EntityModelBuilder toEntityBuilder = _entityBuilders[navigation.To.Type];

      IEnumerable<Name> foreignKeyNames = GetForeignKeyNames(navigation, navigationBuilder, fromEntityBuilder);
      fromEntityBuilder.AddNavigation(navigation, Array.Empty<Name>());
      toEntityBuilder.AddNavigation(navigation.Inverse, foreignKeyNames);
    }

    foreach (EntityModelBuilder entityBuilder in _entityBuilders.Values)
    {
      entityBuilder.AddAccessibleColumns();
    }

    foreach (EntityModelBuilder entityBuilder in _entityBuilders.Values)
    {
      entityBuilder.AddHiddenColumns();
    }

    return dataModel;

    static IEnumerable<Name> GetForeignKeyNames(NavigationModel navigation, NavigationModelBuilder navigationBuilder, EntityModelBuilder fromEntityBuilder)
    {
      if (navigationBuilder.ForeignKeyNames is { Count: > 0 } foreignKeyNames)
        return foreignKeyNames;

      return fromEntityBuilder.PrimaryKeyMembers
        .Select(member => new Name(new ColumnName(navigation.HasMember
          ? $"{navigation.Member.Name}{member.Name}"
          : $"{navigation.From.Type.Name}{member.Name}")));
    }
  }

  public EntityModelBuilder<TEntity> GetEntityBuilder<TEntity>()
    where TEntity : class
  {
    if (_entityBuilders.TryGetValue(typeof(TEntity), out EntityModelBuilder? untypedBuilder))
      return (EntityModelBuilder<TEntity>)untypedBuilder;

    EntityModelBuilder<TEntity> builder = new();
    _entityBuilders.Add(typeof(TEntity), builder);

    return builder;
  }

  public void AddNavigationBuilder<TFrom, TTo>(NavigationModelBuilder<TFrom, TTo> navigationBuilder)
    where TFrom : class
    where TTo : class
  {
    if (_navigationBuilders.Contains(navigationBuilder))
      throw new ModelConfigurationException($"Duplicate association '{typeof(TFrom).Name}' -> '{typeof(TTo).Name}'.");

    _navigationBuilders.Add(navigationBuilder);
  }

  public int GetNextId()
  {
    return ++_navigationId;
  }
}
