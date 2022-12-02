using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;
using static CodeArchitects.Platform.Data.AdoNet.Fixtures.Models.WithDifferentPrimaryKeys;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

public class CommandBuilderTests
{
  private readonly CommandBuilder _sut;

  public CommandBuilderTests()
  {
    _sut = new(Mock.Of<ISqlTextBuilder>());
  }

  [Fact]
  public void BuildSelectCommand_ShouldCreateCorrectParameters_WhenEntityHasSimpleKey()
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

    NavigationSpec<SimpleEntity, int> spec = new(CreateSimpleEntityModel(false), Array.Empty<INavigation>());

    // Act
    _sut.BuildSelectCommand(commandMock.Object, id, spec);

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
  public void BuildSelectCommand_ShouldCreateCorrectParameters_WhenEntityHasCompositeKey()
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

    NavigationSpec<CompositeEntity, (Guid, int)> spec = new(CreateCompositeEntityModel(false), Array.Empty<INavigation>());

    // Act
    _sut.BuildSelectCommand(commandMock.Object, (id1, id2), spec);

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
}
