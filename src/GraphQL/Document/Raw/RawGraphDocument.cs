using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Document.Syntax;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal class RawGraphDocument<TResult> : GraphDocument<TResult>
  where TResult : class
{
  private readonly string _document;

  public RawGraphDocument(string document)
  {
    _document = document;
  }

  public override OperationType OperationType
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      return RawGraphDocument.GetOperationType(ref lexer);
    }
  }

  public override string Name
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      _ = RawGraphDocument.GetOperationType(ref lexer);
      return RawGraphDocument.GetName(ref lexer).ToString();
    }
  }

  public override IDocumentNode CreateDocumentNode(IGraphDocumentContext context)
  {
    // TODO: Validate TResult
    return new RawNode(_document);
  }
}

internal class RawGraphDocument<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TResult : class
  where TVariables : notnull
{
  private readonly string _document;

  public RawGraphDocument(string document)
  {
    _document = document;
  }

  public override OperationType OperationType
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      return RawGraphDocument.GetOperationType(ref lexer);
    }
  }

  public override string Name
  {
    get
    {
      GraphQLLexer lexer = new(_document);
      _ = RawGraphDocument.GetOperationType(ref lexer);
      return RawGraphDocument.GetName(ref lexer).ToString();
    }
  }

  protected override IDocumentNode CreateDocumentNode(IGraphDocumentContext context, IReadOnlyList<IVariable> variables)
  {
    // TODO: Validate TResult
    // TODO: Validate TVariables
    return new RawNode(_document);
  }
}

internal static class RawGraphDocument
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
