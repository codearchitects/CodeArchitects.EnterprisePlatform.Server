using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Literal;

internal class LiteralGraphDocument<TResult> : GraphDocument<TResult>
  where TResult : class
{
  private readonly string _document;

  public LiteralGraphDocument(string document)
  {
    _document = document;
  }

  public override OperationType OperationType
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      return LiteralGraphDocument.GetOperationType(ref lexer);
    }
  }

  public override string Name
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      _ = LiteralGraphDocument.GetOperationType(ref lexer);
      return LiteralGraphDocument.GetName(ref lexer).ToString();
    }
  }

  public override IDocumentNode CreateDocumentNode(IGraphDocumentContext context)
  {
    // TODO: Validate TResult
    return new LiteralNode(_document);
  }
}

internal class LiteralGraphDocument<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TResult : class
  where TVariables : notnull
{
  private readonly string _document;

  public LiteralGraphDocument(string document)
  {
    _document = document;
  }

  public override OperationType OperationType
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      return LiteralGraphDocument.GetOperationType(ref lexer);
    }
  }

  public override string Name
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      _ = LiteralGraphDocument.GetOperationType(ref lexer);
      return LiteralGraphDocument.GetName(ref lexer).ToString();
    }
  }

  protected override IDocumentNode CreateDocumentNode(IGraphDocumentContext context, IReadOnlyList<IVariable> variables)
  {
    // TODO: Validate TResult
    // TODO: Validate TVariables
    return new LiteralNode(_document);
  }
}

internal static class LiteralGraphDocument
{
  public static OperationType GetOperationType(ref GraphQLLexer lexer)
  {
    if (lexer.TokenKind is TokenKind.LeftBrace)
      return OperationType.Query;

    Expect(in lexer, TokenKind.Name);

    var value = lexer.Value;
    lexer.MoveNext();

    return value switch
    {
      "query"        => OperationType.Query,
      "mutation"     => OperationType.Mutation,
      "subscription" => OperationType.Subscription,
      _              => throw new InvalidGraphQLDocumentException("Expected an operation definition.")
    };
  }

  public static ReadOnlySpan<char> GetName(ref GraphQLLexer lexer)
  {
    if (lexer.TokenKind is not TokenKind.Name)
      return ReadOnlySpan<char>.Empty;

    var value = lexer.Value;
    lexer.MoveNext();

    return value;
  }

  public static void Expect(in GraphQLLexer lexer, TokenKind expected)
  {
    if (lexer.TokenKind != expected)
      throw Unexpected(in lexer);
  }

  public static InvalidGraphQLDocumentException Unexpected(in GraphQLLexer lexer)
  {
    TokenKind kind = lexer.TokenKind;
    if (kind is TokenKind.Error)
      throw new InvalidGraphQLDocumentException($"GraphQL document had errors: '{lexer.Error}'.");

    return new InvalidGraphQLDocumentException($"Unexpected token '{kind}'.");
  }
}
