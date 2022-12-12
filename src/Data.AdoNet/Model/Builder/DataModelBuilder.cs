using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

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
    List<NavigationModel> navigations = new();

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

      switch (navigation)
      {
        case SimpleNavigationModel simpleNavigation:
          IEnumerable<Name> foreignKeyNames = GetForeignKeyNames(simpleNavigation, navigationBuilder, fromEntityBuilder);
          fromEntityBuilder.AddSimpleNavigation(simpleNavigation, Array.Empty<Name>());
          toEntityBuilder.AddSimpleNavigation(simpleNavigation.Inverse, foreignKeyNames);
          break;
        case SkipNavigationModel skipNavigation:
          IEnumerable<Name> columnNames = GetColumnNames(navigationBuilder, fromEntityBuilder, toEntityBuilder);
          fromEntityBuilder.AddSkipNavigation(skipNavigation, columnNames);
          break;
        default:
          throw Errors.Unreacheable;
      };

      navigations.Add(navigation);
    }

    foreach (EntityModelBuilder entityBuilder in _entityBuilders.Values)
    {
      entityBuilder.AddPrimaryKeyColumns();
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

    static IEnumerable<Name> GetForeignKeyNames(SimpleNavigationModel navigation, NavigationModelBuilder navigationBuilder, EntityModelBuilder fromEntityBuilder)
    {
      if (navigationBuilder.ForeignKeyNames is { Count: > 0 } foreignKeyNames)
        return foreignKeyNames;

      return fromEntityBuilder.PrimaryKeyMembers
        .Select(member => new Name(new ColumnName(navigation.HasMember
          ? $"{navigation.Member.Name}{member.Name}"
          : $"{navigation.From.Type.Name}{member.Name}")));
    }

    static IEnumerable<Name> GetColumnNames(NavigationModelBuilder navigationBuilder, EntityModelBuilder fromEntityBuilder, EntityModelBuilder toEntityBuilder)
    {
      if (navigationBuilder.ForeignKeyNames is { Count: > 0 } foreignKeyNames)
        return foreignKeyNames;

      IEnumerable<Name> fromColumnNames = fromEntityBuilder.PrimaryKeyMembers
        .Select(member => new Name(new ColumnName($"{fromEntityBuilder.EntityType.Name}{member.Name}")));
      
      IEnumerable<Name> toColumnNames = toEntityBuilder.PrimaryKeyMembers
        .Select(member => new Name(new ColumnName($"{toEntityBuilder.EntityType.Name}{member.Name}")));

      return fromColumnNames.Concat(toColumnNames);
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
