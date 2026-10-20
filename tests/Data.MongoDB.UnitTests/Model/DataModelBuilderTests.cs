using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model;

public class DataModelBuilderTests
{
  [Fact]
  public void AddEntitiesFrom_ShouldSkipTheTypesThatCannotBeLoaded()
  {
    // Arrange
    // One type of the assembly has a missing dependency: GetTypes throws for the whole assembly,
    // and reports the types it could load, with null in place of the others.
    Assembly assembly = new PartiallyLoadableAssembly(typeof(DiscoverableEntity), null);
    DataModelBuilder sut = new();

    // Act
    DataModel model = sut.AddEntitiesFrom(assembly).Build();

    // Assert
    model.TryGetEntity(typeof(DiscoverableEntity), out _).Should().BeTrue();
  }

  [Fact]
  public void GetLoadableTypes_ShouldReturnEveryType_WhenTheWholeAssemblyLoads()
  {
    // Arrange
    Assembly assembly = typeof(DiscoverableEntity).Assembly;

    // Act
    IEnumerable<Type> types = DataModelBuilder.GetLoadableTypes(assembly);

    // Assert
    types.Should().BeEquivalentTo(assembly.GetTypes());
  }

  private sealed class PartiallyLoadableAssembly(params Type?[] types) : Assembly
  {
    public override Type[] GetTypes() =>
      throw new ReflectionTypeLoadException(types, [new TypeLoadException("A dependency is missing.")]);
  }
}
