using CodeArchitects.Platform.GraphQL.Document.Literal;

const string document = """
    query MyQuery($id: ID!, $name: String!) {
      droid(id: $id) @my_directive(arg1: "arg1-value", arg2: -0.123e+45) {
        name # this is a comment
        age
        metal
        ...other
      }
    } # end with a comment
    """;

GraphQLLexer lexer = new(document);

while (lexer.MoveNext())
{
  Console.Write(lexer.TokenKind.ToString().PadRight(20));
  if (lexer.Value is { Length: > 0 } value)
  {
    Console.Write(value.ToString());
  }
  Console.WriteLine();
}
