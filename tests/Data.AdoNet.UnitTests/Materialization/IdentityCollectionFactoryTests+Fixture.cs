using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;
using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class IdentityCollectionFactoryTests
{
  public class ModelDataAttribute : DataAttribute
  {
    private readonly CollectionKind _collectionKind;

    public ModelDataAttribute(CollectionKind collectionKind)
    {
      _collectionKind = collectionKind;
    }

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      IEntityModel entity = EntityModelBuilder.Build(_ => _
        .SetType(typeof(Entity))
        .SetPrimaryKey(_ => _
          .SetType(typeof(int))))
        .Mocked<Entity, int>();

      INavigationModel navigation = SimpleAccessibleNavigationModelBuilder.Build(_ => _
        .SetCollectionKind(_collectionKind)
        .SetTo(entity));

      yield return new object?[] { navigation, entity };
    }
  }

  public class Entity
  {
    public int Id { get; set; }
  }
}
