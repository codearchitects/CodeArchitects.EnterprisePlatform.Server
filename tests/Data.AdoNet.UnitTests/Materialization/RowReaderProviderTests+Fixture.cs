using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;
using System.Reflection;
using Xunit.Sdk;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.Models.WithDifferentPrimaryKeys;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class RowReaderProviderTests
{
  public class EntityWithConstructor
  {
    public EntityWithConstructor(int id)
    {
      Id = id;
    }

    public int Id { get; set; }
    public string? Name { get; set; }
  }

  public class ModelDataAttribute : DataAttribute
  {
    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      PropertyInfo idPropertyInfo = typeof(EntityWithConstructor).GetRequiredProperty(nameof(EntityWithConstructor.Id), BindingFlags.Instance | BindingFlags.Public);
      PropertyInfo namePropertyInfo = typeof(EntityWithConstructor).GetRequiredProperty(nameof(EntityWithConstructor.Name), BindingFlags.Instance | BindingFlags.Public);

      IStandardPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetType(typeof(int))
        .SetIndex(0)
        .SetHasMember(true)
        .SetProperty(idPropertyInfo)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(idPropertyInfo)));

      IOrdinaryColumnModel nameProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetType(typeof(string))
        .SetIndex(1)
        .SetHasMember(true)
        .SetProperty(namePropertyInfo)
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(namePropertyInfo)));

      IEntityModel entityWithConstructor = EntityModelBuilder.Build(_ => _
        .SetType(typeof(EntityWithConstructor))
        .SetColumns(idProperty, nameProperty)
        .SetPrimaryKey(_ => _
          .SetIsComposite(false)
          .SetType(typeof(int))
          .SetColumns(idProperty))
        .SetNavigations()
        .SetInitializer(_ => _
          .SetConstructor(typeof(EntityWithConstructor).GetRequiredConstructor(BindingFlags.Instance | BindingFlags.Public))
          .SetConstructorProperties(idProperty)
          .SetInitializerProperties(nameProperty)));

      yield return new object?[] { CreateSimpleEntityModel(false) };
      yield return new object?[] { CreateCompositeEntityModel(false) };
      yield return new object?[] { entityWithConstructor };
    }
  }
}
