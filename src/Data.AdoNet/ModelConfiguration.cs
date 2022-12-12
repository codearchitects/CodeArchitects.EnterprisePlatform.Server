using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.Builder;
using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet;

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

  public ModelConfiguration()
  {
    _modelBuilder = new();
  }

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

  protected void Entity<TEntity>(Action<IEntityModelBuilder<TEntity>>? buildAction = null)
    where TEntity : class
  {
    EntityModelBuilder<TEntity> entityBuilder = _modelBuilder.GetEntityBuilder<TEntity>();
    buildAction?.Invoke(entityBuilder);
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

  private void Association<TFrom, TTo>(AssociationKind kind, Action<AssociationBuilder<TFrom, TTo>> buildAction)
    where TFrom : class
    where TTo : class
  {
    AssociationBuilder<TFrom, TTo> builder = new(_modelBuilder, kind);
    buildAction(builder);

    _modelBuilder.AddNavigationBuilder<TFrom, TTo>(builder.NavigationBuilder);
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
