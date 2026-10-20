using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace CodeArchitects.Platform.Data.MongoDB.Serialization;

public class MongoDBSerializationInitializerTests
{
  // The class map registry is process-wide: every key test uses types of its own, never
  // serialized anywhere else, and none is marked [Collection] so assembly discovery skips them.
  public class KeyedByTypeName
  {
    public Guid KeyedByTypeNameId { get; set; }
    public string? Name { get; set; }
  }

  public class KeyedOnBaseBase
  {
    public Guid KeyedOnBaseId { get; set; }
  }

  public class KeyedOnBase : KeyedOnBaseBase
  {
    public string? Name { get; set; }
  }

  public class MappedBeforeAddData
  {
    public Guid MappedBeforeAddDataId { get; set; }
  }

  public class KeyedByIdMappedBeforeAddData
  {
    public Guid Id { get; set; }
  }

  private static void MapKeys(params Type[] types)
  {
    DataModelBuilder builder = new();
    foreach (Type type in types)
    {
      builder.AddEntity(type);
    }

    MongoDBSerializationInitializer.MapKeys(builder.Build());
  }

  #region Keys

  [Fact]
  public void MapKeys_ShouldMapATypeNameIdKeyToTheIdElement()
  {
    // Arrange
    MapKeys(typeof(KeyedByTypeName));
    KeyedByTypeName entity = new() { KeyedByTypeNameId = Guid.NewGuid(), Name = "name" };

    // Act
    BsonDocument document = entity.ToBsonDocument();

    // Assert
    document.Names.Should().Contain("_id").And.NotContain(nameof(KeyedByTypeName.KeyedByTypeNameId));
    BsonClassMap.LookupClassMap(typeof(KeyedByTypeName)).IdMemberMap!.MemberName
      .Should().Be(nameof(KeyedByTypeName.KeyedByTypeNameId));
  }

  [Fact]
  public void MapKeys_ShouldGenerateATypeNameIdKeyAsTheDriverDoesForId()
  {
    // Act
    MapKeys(typeof(KeyedByTypeName));

    // Assert
    // Same id generator as a key called Id: the convention only changes which member is the key.
    BsonClassMap.LookupClassMap(typeof(KeyedByTypeName)).IdMemberMap!.IdGenerator
      .Should().BeOfType<GuidGenerator>();
  }

  [Fact]
  public void MapKeys_ShouldMapAKeyDeclaredOnABaseType()
  {
    // Arrange
    MapKeys(typeof(KeyedOnBase));
    KeyedOnBase entity = new() { KeyedOnBaseId = Guid.NewGuid() };

    // Act
    BsonDocument document = entity.ToBsonDocument();

    // Assert
    document.Names.Should().Contain("_id").And.NotContain(nameof(KeyedOnBase.KeyedOnBaseId));
  }

  [Fact]
  public void MapKeys_ShouldThrow_WhenTheClassMapWasCreatedBeforeWithoutTheKey()
  {
    // Arrange
    // Serializing the type freezes a class map built before the provider could map the key.
    BsonClassMap.LookupClassMap(typeof(MappedBeforeAddData));

    // Act
    Action act = () => MapKeys(typeof(MappedBeforeAddData));

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage($"*{nameof(MappedBeforeAddData.MappedBeforeAddDataId)}*");
  }

  [Fact]
  public void MapKeys_ShouldAccept_WhenTheClassMapWasCreatedBeforeWithTheSameKey()
  {
    // Arrange
    BsonClassMap.LookupClassMap(typeof(KeyedByIdMappedBeforeAddData));

    // Act
    Action act = () => MapKeys(typeof(KeyedByIdMappedBeforeAddData));

    // Assert
    act.Should().NotThrow();
  }

  [Fact]
  public void MapKeys_ShouldLeaveABsonIdKeyAsTheDriverMapsIt()
  {
    // Act
    MapKeys(typeof(EntityWithBsonIdAttribute));

    // Assert
    BsonClassMap.LookupClassMap(typeof(EntityWithBsonIdAttribute)).IdMemberMap!.MemberName
      .Should().Be(nameof(EntityWithBsonIdAttribute.Identifier));
  }

  #endregion

  #region Guid serializer

  [Fact]
  public void RegisterGuidSerializer_ShouldRegisterTheConfiguredRepresentation_WhenNoneIsRegistered()
  {
    // Arrange
    BsonSerializerRegistry registry = new();

    // Act
    MongoDBSerializationInitializer.RegisterGuidSerializer(registry, GuidRepresentation.Standard);

    // Assert
    registry.GetSerializer(typeof(Guid)).Should().BeOfType<GuidSerializer>()
      .Which.GuidRepresentation.Should().Be(GuidRepresentation.Standard);
  }

  [Fact]
  public void RegisterGuidSerializer_ShouldKeepTheApplicationSerializer_WhenItHasTheSameRepresentation()
  {
    // Arrange
    BsonSerializerRegistry registry = new();
    GuidSerializer registered = new(GuidRepresentation.Standard);
    registry.RegisterSerializer(typeof(Guid), registered);

    // Act
    Action act = () => MongoDBSerializationInitializer.RegisterGuidSerializer(registry, GuidRepresentation.Standard);

    // Assert
    act.Should().NotThrow();
    registry.GetSerializer(typeof(Guid)).Should().BeSameAs(registered);
  }

  [Fact]
  public void RegisterGuidSerializer_ShouldThrow_WhenTheApplicationSerializerHasAnotherRepresentation()
  {
    // Arrange
    BsonSerializerRegistry registry = new();
    registry.RegisterSerializer(typeof(Guid), new GuidSerializer(GuidRepresentation.CSharpLegacy));

    // Act
    Action act = () => MongoDBSerializationInitializer.RegisterGuidSerializer(registry, GuidRepresentation.Standard);

    // Assert
    act.Should().Throw<InvalidOperationException>()
      .WithMessage("*GuidRepresentation.CSharpLegacy*UseGuidRepresentation*");
  }

  [Fact]
  public void RegisterGuidSerializer_ShouldKeepTheApplicationSerializer_WhenItIsNotBinary()
  {
    // Arrange
    BsonSerializerRegistry registry = new();
    GuidSerializer registered = new(BsonType.String);
    registry.RegisterSerializer(typeof(Guid), registered);

    // Act
    Action act = () => MongoDBSerializationInitializer.RegisterGuidSerializer(registry, GuidRepresentation.Standard);

    // Assert
    // The representation setting only describes binary Guids: a string serializer is used as it is.
    act.Should().NotThrow();
    registry.GetSerializer(typeof(Guid)).Should().BeSameAs(registered);
  }

  #endregion
}
