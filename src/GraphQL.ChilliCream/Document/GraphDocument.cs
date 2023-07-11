using StrawberryShake;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

internal class GraphDocument<TResult, TVariables> : GraphQL.Document.GraphDocument<TResult, TVariables>, IGraphDocument<TResult>, IGraphDocument<TResult, TVariables>, IDocument
  where TVariables : notnull
{
  private readonly string _name;
  private readonly byte[] _content;
  private readonly string _id;

  public GraphDocument(OperationKind kind, string name, byte[] content, string id)
  {
    Kind = kind;
    _name = name;
    _content = content;
    _id = id;
  }

  public OperationKind Kind { get; }

  public ReadOnlySpan<byte> Body => _content;

  public DocumentHash Hash => new DocumentHash("md5Hash", _id);

  public OperationRequest CreateRequest(RequestStrategy strategy)
  {
    return CreateRequestCore(null, null, strategy);
  }

  public OperationRequest CreateRequest(IReadOnlyDictionary<string, object?> variables, IReadOnlyDictionary<string, Upload?> files, RequestStrategy strategy)
  {
    return CreateRequestCore(variables, files, strategy);
  }

  private OperationRequest CreateRequestCore(IReadOnlyDictionary<string, object?>? variables, IReadOnlyDictionary<string, Upload?>? files, RequestStrategy strategy)
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
}
