using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using CodeArchitects.Platform.Data.MongoDB.Serialization;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Filters;

public class FilterProviderTests
{
  static FilterProviderTests() => MongoDBSerializationInitializer.EnsureInitialized();

  private readonly FilterProvider _sut = new();

  private static BsonDocument Render<TEntity>(FilterDefinition<TEntity> filter)
  {
    return filter
      .Render(new RenderArgs<TEntity>(
        BsonSerializer.SerializerRegistry.GetSerializer<TEntity>(),
        BsonSerializer.SerializerRegistry));
  }

  [Fact]
  public void ById_ShouldFilterOnIdElement_WhenKeyPropertyIsCalledId()
  {
    // Arrange
    Guid key = Guid.NewGuid();

    // Act
    BsonDocument filter = Render(_sut.ById<EntityWithIdProperty, Guid>(Models.EntityWithIdProperty, key));

    // Assert
    filter.Names.Should().ContainSingle().Which.Should().Be("_id");
  }

  [Fact]
  public void ById_ShouldFilterOnIdElement_WhenKeyPropertyHasAnotherName()
  {
    // Arrange
    Guid key = Guid.NewGuid();

    // Act
    BsonDocument filter = Render(_sut.ById<EntityWithBsonIdAttribute, Guid>(Models.EntityWithBsonIdAttribute, key));

    // Assert
    // The CLR property is called 'Identifier': the filter must still target '_id',
    // which is what the driver actually stores the key in.
    filter.Names.Should().ContainSingle().Which.Should().Be("_id");
    filter.Names.Should().NotContain("Identifier");
  }

  [Fact]
  public void ByEntity_ShouldFilterOnTheEntityKey()
  {
    // Arrange
    EntityWithIdProperty entity = new() { Id = Guid.NewGuid() };

    // Act
    BsonDocument filter = Render(_sut.ByEntity<EntityWithIdProperty, Guid>(Models.EntityWithIdProperty, entity));
    BsonDocument expected = Render(_sut.ById<EntityWithIdProperty, Guid>(Models.EntityWithIdProperty, entity.Id));

    // Assert
    filter.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void ByEntity_ShouldThrow_WhenEntityIsNull()
  {
    // Act
    Func<FilterDefinition<EntityWithIdProperty>> act =
      () => _sut.ByEntity<EntityWithIdProperty, Guid>(Models.EntityWithIdProperty, null!);

    // Assert
    act.Should().Throw<ArgumentNullException>();
  }

  [Fact]
  public void ById_ShouldThrow_WhenModelIsNull()
  {
    // Act
    Func<FilterDefinition<EntityWithIdProperty>> act =
      () => _sut.ById<EntityWithIdProperty, Guid>(null!, Guid.NewGuid());

    // Assert
    act.Should().Throw<ArgumentNullException>();
  }
}
