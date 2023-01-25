using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.Models.WithDifferentPrimaryKeys;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

public class CommandBuilderTests
{
  private readonly CommandBuilder<IDbCommand> _sut;

  public CommandBuilderTests()
  {
    _sut = new(Mock.Of<ISqlTextBuilder>());
  }

  [Fact]
  public void BuildFindCommand_ShouldCreateCorrectParameters_WhenEntityHasSimpleKey()
  {
    // Arrange
    int id = 3;
    Mock<IDbDataParameter> idParameterMock = new(MockBehavior.Loose);
    Mock<IDbCommand> commandMock = new(MockBehavior.Loose);

    commandMock
      .SetupSequence(x => x.CreateParameter())
      .Returns(idParameterMock.Object);
    commandMock
      .Setup(x => x.Parameters.Add(idParameterMock.Object))
      .Returns(1);

    IEntityModel<SimpleEntity, int> model = CreateSimpleEntityModel(false);
    FakeNavigationRoot<SimpleEntity, int> root = new(model, Array.Empty<INavigation>());

    // Act
    _sut.BuildFindCommand(commandMock.Object, id, root);

    // Assert
    commandMock.VerifySet(command => command.CommandText = null);
    commandMock.Verify(x => x.CreateParameter(), Times.Once);
    commandMock.Verify(x => x.Parameters.Add(idParameterMock.Object), Times.Once);
    commandMock.VerifyNoOtherCalls();
    
    idParameterMock.VerifySet(parameter => parameter.ParameterName = "p0", Times.Once);
    idParameterMock.VerifySet(parameter => parameter.Value = id, Times.Once);
    idParameterMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void BuildFindCommand_ShouldCreateCorrectParameters_WhenEntityHasCompositeKey()
  {
    // Arrange
    Guid id1 = Guid.NewGuid();
    int id2 = 3;
    Mock<IDbDataParameter> id1ParameterMock = new(MockBehavior.Loose);
    Mock<IDbDataParameter> id2ParameterMock = new(MockBehavior.Loose);
    Mock<IDbCommand> commandMock = new(MockBehavior.Loose);

    commandMock
      .SetupSequence(x => x.CreateParameter())
      .Returns(id1ParameterMock.Object)
      .Returns(id2ParameterMock.Object);
    commandMock
      .Setup(x => x.Parameters.Add(id1ParameterMock.Object))
      .Returns(1);
    commandMock
      .Setup(x => x.Parameters.Add(id2ParameterMock.Object))
      .Returns(2);

    IEntityModel<CompositeEntity, (Guid, int)> model = CreateCompositeEntityModel(false);
    FakeNavigationRoot<CompositeEntity, (Guid, int)> root = new(model, Array.Empty<INavigation>());

    // Act
    _sut.BuildFindCommand(commandMock.Object, (id1, id2), root);

    // Assert
    commandMock.VerifySet(command => command.CommandText = null);
    commandMock.Verify(x => x.CreateParameter(), Times.Exactly(2));
    commandMock.Verify(x => x.Parameters.Add(id1ParameterMock.Object), Times.Once);
    commandMock.Verify(x => x.Parameters.Add(id2ParameterMock.Object), Times.Once);
    commandMock.VerifyNoOtherCalls();
    
    id1ParameterMock.VerifySet(parameter => parameter.ParameterName = "p0", Times.Once);
    id1ParameterMock.VerifySet(parameter => parameter.Value = id1, Times.Once);
    id1ParameterMock.VerifyNoOtherCalls();
    id2ParameterMock.VerifySet(parameter => parameter.ParameterName = "p1", Times.Once);
    id2ParameterMock.VerifySet(parameter => parameter.Value = id2, Times.Once);
    id2ParameterMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void BuildInsertCommand_ShouldCreateCorrectParameters_WhenEntityHasSimpleKey()
  {
    // Arrange
    Mock<IDbDataParameter> idParameterMock = new(MockBehavior.Loose);
    Mock<IDbDataParameter> nameParameterMock = new(MockBehavior.Loose);
    Mock<IDbCommand> commandMock = new(MockBehavior.Loose);

    commandMock
      .SetupSequence(x => x.CreateParameter())
      .Returns(idParameterMock.Object)
      .Returns(nameParameterMock.Object);
    commandMock
      .Setup(x => x.Parameters.Add(idParameterMock.Object))
      .Returns(1);
    commandMock
      .Setup(x => x.Parameters.Add(nameParameterMock.Object))
      .Returns(2);

    IEntityModel<SimpleEntity, int> model = CreateSimpleEntityModel(false);
    SimpleEntity entity = SimpleEntity.One();

    // Act
    _sut.BuildInsertCommand(commandMock.Object, entity, model, default);

    // Assert
    commandMock.Verify(command => command.Parameters.Clear());
    commandMock.VerifySet(command => command.CommandText = null);
    commandMock.Verify(x => x.CreateParameter(), Times.Exactly(2));
    commandMock.Verify(x => x.Parameters.Add(idParameterMock.Object), Times.Once);
    commandMock.Verify(x => x.Parameters.Add(nameParameterMock.Object), Times.Once);
    commandMock.VerifyNoOtherCalls();
    
    idParameterMock.VerifySet(parameter => parameter.ParameterName = "p0", Times.Once);
    idParameterMock.VerifySet(parameter => parameter.Value = entity.Id, Times.Once);
    idParameterMock.VerifyNoOtherCalls();
    
    nameParameterMock.VerifySet(parameter => parameter.ParameterName = "p1", Times.Once);
    nameParameterMock.VerifySet(parameter => parameter.Value = entity.Name, Times.Once);
    nameParameterMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void BuildInsertCommand_ShouldCreateCorrectParameters_WhenEntityHasSimpleAutoGeneratedKey()
  {
    // Arrange
    Mock<IDbDataParameter> nameParameterMock = new(MockBehavior.Loose);
    Mock<IDbCommand> commandMock = new(MockBehavior.Loose);

    commandMock
      .SetupSequence(x => x.CreateParameter())
      .Returns(nameParameterMock.Object);
    commandMock
      .Setup(x => x.Parameters.Add(nameParameterMock.Object))
      .Returns(1);

    IEntityModel<SimpleEntity, int> model = CreateSimpleEntityModel(true);
    SimpleEntity entity = SimpleEntity.One();

    // Act
    _sut.BuildInsertCommand(commandMock.Object, entity, model, default);

    // Assert
    commandMock.Verify(command => command.Parameters.Clear());
    commandMock.VerifySet(command => command.CommandText = null);
    commandMock.Verify(x => x.CreateParameter(), Times.Once);
    commandMock.Verify(x => x.Parameters.Add(nameParameterMock.Object), Times.Once);
    commandMock.VerifyNoOtherCalls();
    
    nameParameterMock.VerifySet(parameter => parameter.ParameterName = "p1", Times.Once);
    nameParameterMock.VerifySet(parameter => parameter.Value = entity.Name, Times.Once);
    nameParameterMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void BuildInsertCommand_ShouldCreateCorrectParameters_WhenEntityHasCompositeKey()
  {
    // Arrange
    Mock<IDbDataParameter> id1ParameterMock = new(MockBehavior.Loose);
    Mock<IDbDataParameter> id2ParameterMock = new(MockBehavior.Loose);
    Mock<IDbDataParameter> nameParameterMock = new(MockBehavior.Loose);
    Mock<IDbCommand> commandMock = new(MockBehavior.Loose);

    commandMock
      .SetupSequence(x => x.CreateParameter())
      .Returns(id1ParameterMock.Object)
      .Returns(id2ParameterMock.Object)
      .Returns(nameParameterMock.Object);
    commandMock
      .Setup(x => x.Parameters.Add(id1ParameterMock.Object))
      .Returns(1);
    commandMock
      .Setup(x => x.Parameters.Add(id2ParameterMock.Object))
      .Returns(2);
    commandMock
      .Setup(x => x.Parameters.Add(nameParameterMock.Object))
      .Returns(3);

    IEntityModel<CompositeEntity, (Guid, int)> model = CreateCompositeEntityModel(false);
    CompositeEntity entity = CompositeEntity.One();

    // Act
    _sut.BuildInsertCommand(commandMock.Object, entity, model, default);

    // Assert
    commandMock.Verify(command => command.Parameters.Clear());
    commandMock.VerifySet(command => command.CommandText = null);
    commandMock.Verify(x => x.CreateParameter(), Times.Exactly(3));
    commandMock.Verify(x => x.Parameters.Add(id1ParameterMock.Object), Times.Once);
    commandMock.Verify(x => x.Parameters.Add(id2ParameterMock.Object), Times.Once);
    commandMock.Verify(x => x.Parameters.Add(nameParameterMock.Object), Times.Once);
    commandMock.VerifyNoOtherCalls();

    id1ParameterMock.VerifySet(parameter => parameter.ParameterName = "p0", Times.Once);
    id1ParameterMock.VerifySet(parameter => parameter.Value = entity.Id1, Times.Once);
    id1ParameterMock.VerifyNoOtherCalls();

    id2ParameterMock.VerifySet(parameter => parameter.ParameterName = "p1", Times.Once);
    id2ParameterMock.VerifySet(parameter => parameter.Value = entity.Id2, Times.Once);
    id2ParameterMock.VerifyNoOtherCalls();

    nameParameterMock.VerifySet(parameter => parameter.ParameterName = "p2", Times.Once);
    nameParameterMock.VerifySet(parameter => parameter.Value = entity.Name, Times.Once);
    nameParameterMock.VerifyNoOtherCalls();
  }

  [Fact]
  public void BuildInsertCommand_ShouldCreateCorrectParameters_WhenEntityHasCompositeAutoGeneratedKey()
  {
    // Arrange
    Mock<IDbDataParameter> id1ParameterMock = new(MockBehavior.Loose);
    Mock<IDbDataParameter> nameParameterMock = new(MockBehavior.Loose);
    Mock<IDbCommand> commandMock = new(MockBehavior.Loose);

    commandMock
      .SetupSequence(x => x.CreateParameter())
      .Returns(id1ParameterMock.Object)
      .Returns(nameParameterMock.Object);
    commandMock
      .Setup(x => x.Parameters.Add(id1ParameterMock.Object))
      .Returns(1);
    commandMock
      .Setup(x => x.Parameters.Add(nameParameterMock.Object))
      .Returns(2);

    IEntityModel<CompositeEntity, (Guid, int)> model = CreateCompositeEntityModel(true);
    CompositeEntity entity = CompositeEntity.One();

    // Act
    _sut.BuildInsertCommand(commandMock.Object, entity, model, default);

    // Assert
    commandMock.Verify(command => command.Parameters.Clear());
    commandMock.VerifySet(command => command.CommandText = null);
    commandMock.Verify(x => x.CreateParameter(), Times.Exactly(2));
    commandMock.Verify(x => x.Parameters.Add(id1ParameterMock.Object), Times.Once);
    commandMock.Verify(x => x.Parameters.Add(nameParameterMock.Object), Times.Once);
    commandMock.VerifyNoOtherCalls();

    id1ParameterMock.VerifySet(parameter => parameter.ParameterName = "p0", Times.Once);
    id1ParameterMock.VerifySet(parameter => parameter.Value = entity.Id1, Times.Once);
    id1ParameterMock.VerifyNoOtherCalls();

    nameParameterMock.VerifySet(parameter => parameter.ParameterName = "p2", Times.Once);
    nameParameterMock.VerifySet(parameter => parameter.Value = entity.Name, Times.Once);
    nameParameterMock.VerifyNoOtherCalls();
  }
}
