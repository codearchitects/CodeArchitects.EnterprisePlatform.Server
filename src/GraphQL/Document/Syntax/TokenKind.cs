namespace CodeArchitects.Platform.GraphQL.Document.Syntax;

internal enum TokenKind
{
    StartOfFile,      // <SOF>
    EndOfFile,        // <EOF>
    Error,            // N/D
    Bang,             // !
    QuestionMark,     // ?
    Dollar,           // $
    Ampersand,        // &
    LeftParenthesis,  // (
    RightParenthesis, // )
    LeftBracket,      // [
    RightBracket,     // ]
    LeftBrace,        // {
    RightBrace,       // }
    Spread,           // ...
    Colon,            // :
    Equals,           // =
    At,               // @
    Pipe,             // |
    Name,             // name
    String,           // "string"
    BlockString,      // """line1 \n line2"""  // TODO: BlockStringDelimiter + BlockStringLine
    Integer,          // 42
    Float             // 69.420
}
