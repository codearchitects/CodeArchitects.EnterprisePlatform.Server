using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.MongoDB.Model;
using MongoDB.Bson;

namespace CodeArchitects.Platform.Data.MongoDB.Mapping;

public class MongoDBMappedRepositoryTests
{
  private readonly Mock<IDataContext> _context = new();

  private static WidgetDocument ToDocument(Widget widget) => new()
  {
    Id = widget.Id,
    Label = widget.Name,
    Part = new WidgetPartDocument { Size = widget.Part.Size }
  };

  private static Widget ToEntity(WidgetDocument document) => new()
  {
    Id = document.Id,
    Name = document.Label,
    Part = new WidgetPart { Size = document.Part.Size }
  };

  private DelegatingRepository<WidgetDocument, Widget, Guid> CreateRepository(
    Func<Widget, WidgetDocument?>? toDocument = null,
    Func<WidgetDocument, Widget?>? toEntity = null) =>
    new(_context.Object, toDocument ?? ToDocument, toEntity ?? ToEntity);

  private static Widget NewWidget() => new() { Id = Guid.NewGuid(), Name = "widget", Part = new WidgetPart { Size = 3 } };

  #region Delegation

  [Fact]
  public void Insert_ShouldPassTheMappedDocumentToTheDataContext()
  {
    // Arrange
    Widget widget = NewWidget();
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository();

    // Act
    repository.Insert(widget);

    // Assert
    _context.Verify(context => context.Insert<WidgetDocument, Guid>(It.Is<WidgetDocument>(document =>
      document.Id == widget.Id && document.Label == widget.Name && document.Part.Size == widget.Part.Size)), Times.Once);
  }

  [Fact]
  public async Task FindAsync_ShouldMapTheDocumentFromTheDataContext()
  {
    // Arrange
    Widget expected = NewWidget();
    _context
      .Setup(context => context.FindAsync<WidgetDocument, Guid>(expected.Id, It.IsAny<CancellationToken>()))
      .ReturnsAsync(ToDocument(expected));
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository();

    // Act
    Widget? widget = await repository.FindAsync(expected.Id);

    // Assert
    widget.Should().BeEquivalentTo(expected);
  }

  [Fact]
  public void Find_ShouldReturnNull_WithoutMapping_WhenTheDocumentDoesNotExist()
  {
    // Arrange
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository(
      toEntity: _ => throw new InvalidOperationException("must not map"));

    // Act & Assert
    repository.Find(Guid.NewGuid()).Should().BeNull();
  }

  [Fact]
  public void Find_ShouldThrow_WhenAnIncludeIsRequested()
  {
    // Arrange
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository();

    // Act
    Action act = () => repository.Find(Guid.NewGuid(), include => include.Include(widget => widget.Part));

    // Assert
    act.Should().Throw<NotSupportedException>().WithMessage("*Widget*Find(key)*");
    _context.VerifyNoOtherCalls();
  }

  [Fact]
  public void Remove_ByKey_ShouldNotMap()
  {
    // Arrange
    Guid key = Guid.NewGuid();
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository(
      toDocument: _ => throw new InvalidOperationException("must not map"));

    // Act
    repository.Remove(key);

    // Assert
    _context.Verify(context => context.Remove<WidgetDocument, Guid>(key), Times.Once);
  }

  #endregion

  #region Checked mapping

  [Fact]
  public void Insert_ShouldThrow_WhenTheEntityIsNull()
  {
    // Arrange
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository();

    // Act
    Action act = () => repository.Insert(null!);

    // Assert
    act.Should().Throw<ArgumentNullException>().WithParameterName("entity");
  }

  [Fact]
  public void InsertMany_ShouldThrow_WhenAnElementIsNull()
  {
    // Arrange
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository();

    // Act
    Action act = () => repository.InsertMany([NewWidget(), null!]);

    // Assert
    act.Should().Throw<ArgumentException>().WithMessage("*index 1*").WithParameterName("entities");
    _context.VerifyNoOtherCalls();
  }

  [Fact]
  public void Update_ShouldThrow_WhenTheMappingReturnsNull()
  {
    // Arrange
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository(toDocument: _ => null);

    // Act
    Action act = () => repository.Update(NewWidget());

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*'Widget' to 'WidgetDocument' returned null*");
    _context.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task FindAsync_ShouldThrow_WhenTheMappingReturnsNull()
  {
    // Arrange
    _context
      .Setup(context => context.FindAsync<WidgetDocument, Guid>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(new WidgetDocument());
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository(toEntity: _ => null);

    // Act
    Func<Task> act = () => repository.FindAsync(Guid.NewGuid());

    // Assert
    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*'WidgetDocument' to 'Widget' returned null*");
  }

  [Theory]
  [InlineData(nameof(IRepository<Widget, Guid>.Update))]
  [InlineData(nameof(IRepository<Widget, Guid>.Upsert))]
  [InlineData(nameof(IRepository<Widget, Guid>.Remove))]
  [InlineData(nameof(IRepository<Widget, Guid>.Insert))]
  public void Write_ShouldThrow_WhenTheMappingDoesNotPreserveTheKey(string operation)
  {
    // Arrange
    Widget widget = NewWidget();
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository(
      toDocument: entity => new WidgetDocument { Id = Guid.NewGuid(), Label = entity.Name });

    // Act
    Action act = operation switch
    {
      nameof(repository.Update) => () => repository.Update(widget),
      nameof(repository.Upsert) => () => repository.Upsert(widget),
      nameof(repository.Remove) => () => repository.Remove(widget),
      _ => () => repository.Insert(widget)
    };

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*does not preserve the key*Widget.Id*WidgetDocument.Id*");
    _context.VerifyNoOtherCalls();
  }

  [Fact]
  public void UpdateMany_ShouldQueueNothing_WhenOneElementFailsToMap()
  {
    // Arrange
    Widget broken = NewWidget();
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository(
      toDocument: entity => entity == broken ? throw new FormatException("bad") : ToDocument(entity));

    // Act
    Action act = () => repository.UpdateMany([NewWidget(), NewWidget(), broken]);

    // Assert
    // The batch is mapped before the data context sees any of it.
    act.Should().Throw<InvalidOperationException>()
      .WithMessage("*'Widget' to 'WidgetDocument' (element at index 2) failed: bad*")
      .WithInnerException<FormatException>();
    _context.VerifyNoOtherCalls();
  }

  [Fact]
  public void UpdateMany_ShouldNameTheElement_WhenItsKeyIsNotPreserved()
  {
    // Arrange
    Widget broken = NewWidget();
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository(
      toDocument: entity => entity == broken ? new WidgetDocument { Id = Guid.NewGuid() } : ToDocument(entity));

    // Act
    Action act = () => repository.UpdateMany([NewWidget(), broken]);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*(element at index 1) does not preserve the key*");
    _context.VerifyNoOtherCalls();
  }

  [Fact]
  public void Update_ShouldSkipTheKeyCheck_WhenTheEntityKeyCannotBeResolved()
  {
    // Arrange
    Keyless entity = new() { Identifier = Guid.NewGuid() };
    DelegatingRepository<KeylessDocument, Keyless, Guid> repository = new(
      _context.Object,
      keyless => new KeylessDocument { Id = keyless.Identifier },
      document => new Keyless { Identifier = document.Id });

    // Act
    repository.Update(entity);

    // Assert
    _context.Verify(context => context.Update<KeylessDocument, Guid>(It.Is<KeylessDocument>(document => document.Id == entity.Identifier)), Times.Once);
  }

  #endregion

  #region Generated keys

  private DelegatingRepository<NoteDocument, Note, string> CreateNoteRepository() => new(
    _context.Object,
    note => new NoteDocument { Id = note.Id, Text = note.Text },
    document => Note.WithId(document.Id!, document.Text));

  [Fact]
  public void Insert_ShouldGenerateTheKey_AndGiveItToTheEntity()
  {
    // Arrange
    Note note = new("text");
    DelegatingRepository<NoteDocument, Note, string> repository = CreateNoteRepository();

    // Act
    repository.Insert(note);

    // Assert
    // Generated before the data context is called, and written through the private setter.
    ObjectId.TryParse(note.Id, out _).Should().BeTrue();
    _context.Verify(context => context.Insert<NoteDocument, string>(It.Is<NoteDocument>(document => document.Id == note.Id)), Times.Once);
  }

  [Fact]
  public async Task InsertManyAsync_ShouldGenerateADistinctKeyForEachEntity()
  {
    // Arrange
    Note first = new("first");
    Note second = new("second");
    DelegatingRepository<NoteDocument, Note, string> repository = CreateNoteRepository();

    // Act
    await repository.InsertManyAsync([first, second]);

    // Assert
    first.Id.Should().NotBeNullOrEmpty();
    second.Id.Should().NotBeNullOrEmpty().And.NotBe(first.Id);
  }

  [Fact]
  public void Insert_ShouldKeepTheKey_WhenTheEntityAlreadyHasOne()
  {
    // Arrange
    string id = ObjectId.GenerateNewId().ToString();
    Note note = Note.WithId(id, "text");
    DelegatingRepository<NoteDocument, Note, string> repository = CreateNoteRepository();

    // Act
    repository.Insert(note);

    // Assert
    note.Id.Should().Be(id);
    _context.Verify(context => context.Insert<NoteDocument, string>(It.Is<NoteDocument>(document => document.Id == id)), Times.Once);
  }

  [Fact]
  public void Insert_ShouldGiveTheKeyToTheEntity_ThroughAPrivateSetterOfABaseClass()
  {
    // Arrange
    Memo memo = new() { Text = "text" };
    DelegatingRepository<NoteDocument, Memo, string> repository = new(
      _context.Object,
      entity => new NoteDocument { Id = entity.Id, Text = entity.Text },
      document => new Memo { Text = document.Text });

    // Act
    repository.Insert(memo);

    // Assert
    ObjectId.TryParse(memo.Id, out _).Should().BeTrue();
  }

  [Fact]
  public void Insert_ShouldLeaveTheEntityUnchanged_WhenTheDataContextRejectsIt()
  {
    // Arrange
    Note note = new("text");
    _context
      .Setup(context => context.Insert<NoteDocument, string>(It.IsAny<NoteDocument>()))
      .Throws(new InvalidOperationException("rejected"));
    DelegatingRepository<NoteDocument, Note, string> repository = CreateNoteRepository();

    // Act
    Action act = () => repository.Insert(note);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("rejected");
    note.Id.Should().BeNull();
  }

  [Fact]
  public async Task InsertManyAsync_ShouldLeaveEveryEntityUnchanged_WhenOneFailsToMap()
  {
    // Arrange
    Note first = new("first");
    Note broken = new("broken");
    DelegatingRepository<NoteDocument, Note, string> repository = new(
      _context.Object,
      note => note == broken ? throw new FormatException("bad") : new NoteDocument { Id = note.Id, Text = note.Text },
      document => Note.WithId(document.Id!, document.Text));

    // Act
    Func<Task> act = () => repository.InsertManyAsync([first, broken]);

    // Assert
    // The batch is rejected before any key is generated, so the first entity keeps no key either.
    await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*index 1*");
    first.Id.Should().BeNull();
    _context.VerifyNoOtherCalls();
  }

  [Fact]
  public void Write_ShouldUseTheModelRegisteredInTheDataContext()
  {
    // Arrange
    // A data context built by AddData exposes its model: an unregistered document fails with its message.
    _context.As<IEntityModelResolver>()
      .Setup(resolver => resolver.GetEntityModel<WidgetDocument>())
      .Throws(new InvalidOperationException("'WidgetDocument' is not registered as a MongoDB entity."));
    DelegatingRepository<WidgetDocument, Widget, Guid> repository = CreateRepository();

    // Act
    Action act = () => repository.Update(NewWidget());

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*not registered as a MongoDB entity*");
    _context.Verify(context => context.Update<WidgetDocument, Guid>(It.IsAny<WidgetDocument>()), Times.Never);
  }

  [Fact]
  public void Update_ShouldNotGenerateAKey()
  {
    // Arrange
    Note note = new("text");
    DelegatingRepository<NoteDocument, Note, string> repository = CreateNoteRepository();

    // Act
    repository.Update(note);

    // Assert
    note.Id.Should().BeNull();
  }

  #endregion
}
