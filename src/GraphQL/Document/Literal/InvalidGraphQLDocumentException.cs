using System.Runtime.Serialization;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

[Serializable]
public class InvalidGraphQLDocumentException : Exception
{
  public InvalidGraphQLDocumentException()
    : base("The given GraphQL document had syntax errors.")
  {
  }

  public InvalidGraphQLDocumentException(string message)
    : base(message)
  {
  }

  public InvalidGraphQLDocumentException(string message, Exception inner)
    : base(message, inner)
  {
  }

  protected InvalidGraphQLDocumentException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}