using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

namespace CodeArchitects.Platform.Data.MongoDB.Model;

public class EntityModelTests
{
  [Fact]
  public void Create_ShoulCreateCorrectEntityModel_WhenEntityHasIdProperty()
  {
    // Arrange

    // Act
    EntityModel sut = EntityModel.Create(typeof(EntityWithIdProperty));

    // Assert
    sut.Should().BeEquivalentTo(Models.EntityWithIdProperty);
  }

  [Fact]
  public void Create_ShoulCreateCorrectEntityModel_WhenEntityHasIdPropertyAndTableAttribute()
  {
    // Arrange

    // Act
    EntityModel sut = EntityModel.Create(typeof(EntityWithIdPropertyAndTableAttribute));

    // Assert
    sut.Should().BeEquivalentTo(Models.EntityWithIdPropertyAndTableAttribute);
  }

  [Fact]
  public void Create_ShoulCreateCorrectEntityModel_WhenEntityHasBsonIdAttribute()
  {
    // Arrange

    // Act
    EntityModel sut = EntityModel.Create(typeof(EntityWithBsonIdAttribute));

    // Assert
    sut.Should().BeEquivalentTo(Models.EntityWithBsonIdAttribute);
  }

  [Fact]
  public void Create_ShouldThrowException_WhenEntityDoesNotHaveIdPropertyOrBsonIdAttribute()
  {
    // Arrange

    // Act
    Func<EntityModel> act = () => EntityModel.Create(typeof(MalformedEntity));

    // Assert
    act.Should().Throw<InvalidOperationException>();
  }
}
