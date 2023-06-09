using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder.Content;
using System.Globalization;
using System.Text;

namespace CodeArchitects.Platform.GraphQL.Fixtures;

internal class StringContentBuilder : IContentBuilder<string>, IKeywords<string>, IPunctuators<string>, ITrivias<string>
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

  public IContentBuilder<string> Append(string s)
  {
    _sb.Append(s);
    return this;
  }

  public IContentBuilder<string> Append(ReadOnlySpan<char> s)
  {
    _sb.Append(s);
    return this;
  }

  public IContentBuilder<string> Append(string symbol, int repeatCount)
  {
    for (int i = 0; i < repeatCount; i++)
    {
      _sb.Append(symbol);
    }
    return this;
  }

  public IContentBuilder<string> Append(OperationType operationType)
  {
    switch (operationType)
    {
      case OperationType.Query:
        _sb.Append("query");
        return this;
      case OperationType.Mutation:
        _sb.Append("mutation");
        return this;
      case OperationType.Subscription:
        _sb.Append("subscription");
        return this;
    }

    throw new ArgumentException("Invalid operation", nameof(operationType));
  }

  public IContentBuilder<string> AppendCamelized(string s)
  {
    _sb.AppendCamelized(s);
    return this;
  }

  public IContentBuilder<string> AppendLine()
  {
    _sb.AppendLine();
    return this;
  }

  public IContentBuilder<string> AppendLiteral(double d)
  {
    _sb.Append(d.ToString(CultureInfo.InvariantCulture));
    return this;
  }

  public IContentBuilder<string> AppendLiteral(long l)
  {
    _sb.Append(l);
    return this;
  }

  public IContentBuilder<string> AppendLiteral(string s)
  {
    _sb
      .Append('"')
      .Append(s)
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
