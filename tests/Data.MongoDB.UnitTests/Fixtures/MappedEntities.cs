using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

public class Widget
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public WidgetPart Part { get; set; } = new();
}

public class WidgetPart
{
  public int Size { get; set; }
}

public class WidgetDocument
{
  public Guid Id { get; set; }
  public string Label { get; set; } = string.Empty;
  public WidgetPartDocument Part { get; set; } = new();
}

public class WidgetPartDocument
{
  public int Size { get; set; }
}

/// <summary>The domain key has a private setter: a generated key must reach it all the same.</summary>
public class Note
{
  public Note(string text) => Text = text;

  public string? Id { get; private set; }
  public string Text { get; }

  public static Note WithId(string id, string text) => new(text) { Id = id };
}

public class NoteDocument
{
  [BsonRepresentation(BsonType.ObjectId)]
  public string? Id { get; set; }
  public string Text { get; set; } = string.Empty;
}

/// <summary>The usual DDD base class: the key setter is private and declared here, not in the entity.</summary>
public abstract class EntityBase
{
  public string? Id { get; private set; }
}

public class Memo : EntityBase
{
  public string Text { get; set; } = string.Empty;
}

/// <summary>A domain entity without any key the repository can resolve by convention.</summary>
public class Keyless
{
  public Guid Identifier { get; set; }
}

public class KeylessDocument
{
  public Guid Id { get; set; }
}

/// <summary>
/// Maps with the delegates it is given, so that each test can make the mapping misbehave.
/// </summary>
public class DelegatingRepository<TDocument, TEntity, TKey>(
  IDataContext context,
  Func<TEntity, TDocument?> toDocument,
  Func<TDocument, TEntity?> toEntity)
  : MongoDBMappedRepository<TDocument, TEntity, TKey>(context)
  where TDocument : class
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  protected override TDocument EntityToTable(TEntity entity) => toDocument(entity)!;

  protected override TEntity TableToEntity(TDocument document) => toEntity(document)!;
}
