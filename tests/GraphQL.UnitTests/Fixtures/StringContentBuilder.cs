using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Content;
using System.Globalization;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class StringContentBuilder : IDocumentContentBuilder<string>, IKeywords<string>, IPunctuators<string>, ITrivias<string>
{
  private readonly StringBuilder _sb;

  public StringContentBuilder()
  {
    _sb = new();
  }

  public string Content => _sb.ToString();

  public int Length => _sb.Length;

  public IKeywords<string> Keywords => this;

  public IPunctuators<string> Punctuators => this;

  public ITrivias<string> Trivias => this;

  public string Query => "query";

  public string Mutation => "mutation";

  public string Subscription => "subscription";

  public string Null => "null";

  public string True => "true";

  public string False => "false";

  public string Bang => "!";

  public string Colon => ":";

  public string DollarSign => "$";

  public string AtSign => "@";

  public string LeftParenthesis => "(";

  public string RightParenthesis => ")";

  public string LeftBracket => "[";

  public string RightBracket => "]";

  public string LeftBrace => "{";

  public string RightBrace => "}";

  public string Space => " ";

  public string Comma => ",";

  public IDocumentContentBuilder<string> Append(string @string)
  {
    _sb.Append(@string);
    return this;
  }

  public IDocumentContentBuilder<string> Append(string symbol, int repeatCount)
  {
    for (int i = 0; i < repeatCount; i++)
    {
      _sb.Append(symbol);
    }
    return this;
  }

  public IDocumentContentBuilder<string> AppendCamelized(string @string)
  {
    _sb.AppendCamelized(@string);
    return this;
  }

  public IDocumentContentBuilder<string> AppendLine()
  {
    _sb.AppendLine();
    return this;
  }

  public IDocumentContentBuilder<string> AppendLiteral(float number)
  {
    _sb.Append(number.ToString(CultureInfo.InvariantCulture));
    return this;
  }

  public IDocumentContentBuilder<string> AppendLiteral(int number)
  {
    _sb.Append(number);
    return this;
  }

  public IDocumentContentBuilder<string> AppendLiteral(string @string)
  {
    _sb
      .Append('"')
      .Append(@string)
      .Append('"');
    return this;
  }

  public GraphDocument<TResult> GetDocument<TResult>()
  {
    return new StringGraphDocument<TResult>(_sb.ToString());
  }

  public GraphDocument<TResult, TVariables> GetDocument<TResult, TVariables>()
    where TVariables : notnull
  {
    return new StringGraphDocument<TResult, TVariables>(_sb.ToString());
  }

  public void Pop()
  {
    _sb.Remove(_sb.Length - 1, 1);
  }
}
