using CodeArchitects.Platform.GraphQL.Document;
using StrawberryShake;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.ChilliCream.Document;

internal class Utf8Document : IUtf8Document, IDocument
{
  private readonly OperationKind _kind;
  private readonly string _name;
  private readonly byte[] _content;
  private readonly string _id;

  public Utf8Document(OperationKind kind, string name, byte[] content, string id)
  {
    _kind = kind;
    _name = name;
    _content = content;
    _id = id;
  }

  public byte[] Content => _content;

  OperationKind IDocument.Kind => _kind;

  ReadOnlySpan<byte> IDocument.Body => _content;

  DocumentHash IDocument.Hash => new DocumentHash("md5Hash", _id);

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
