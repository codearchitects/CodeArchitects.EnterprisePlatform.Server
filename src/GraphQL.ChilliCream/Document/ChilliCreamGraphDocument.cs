using CodeArchitects.Platform.GraphQL.Document;
using StrawberryShake;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

internal class ChilliCreamGraphDocument<TResult, TVariables> : GraphDocument<TResult, TVariables>, IDocument
  where TVariables : notnull
{
  private readonly string _name;
  private readonly byte[] _content;
  private readonly string _id;

  public ChilliCreamGraphDocument(OperationKind kind, string name, byte[] content, string id)
  {
    Kind = kind;
    _name = name;
    _content = content;
    _id = id;
  }

  public OperationKind Kind { get; }

  public ReadOnlySpan<byte> Body => _content;

  public DocumentHash Hash => new DocumentHash("md5Hash", _id);

  protected OperationRequest CreateRequest(IReadOnlyDictionary<string, object?>? variables, IReadOnlyDictionary<string, Upload?>? files, RequestStrategy strategy)
  {
    return new OperationRequest(
      id: _id,
      name: _name,
      document: this,
      variables: variables,
      files: files,
      strategy: strategy);
  }

  public override string ToString() => Encoding.UTF8.GetString(_content);

  public static ChilliCreamGraphDocument<TResult, TVariables> Create(OperationKind kind, string? name, byte[] content)
  {
    return new(
      kind,
      name ?? string.Empty,
      content,
      Guid.NewGuid().ToString("N"));
  }
}
