using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace CodeArchitects.Platform.Actors.Analyzer;

[DebuggerDisplay("{ToString(),nq}")]
internal sealed record DiagnosticReference(
  string DiagnosticId,
  DiagnosticSeverity? Severity,
  string FilePath,
  TextSpan TextSpan,
  RecordList<string> Properties,
  RecordList<string> Args)
{
  public Location GetLocation(SyntaxTree syntaxTree) => Location.Create(syntaxTree, TextSpan);

  public ImmutableDictionary<string, string?>? GetPropertyDictionary()
  {
    if (Properties.Count == 0)
      return null;

    var builder = ImmutableDictionary.CreateBuilder<string, string?>();
    for (int i = 0; i < Properties.Count; i++)
    {
      builder.Add(i.ToString(), Properties[i]);
    }

    return builder.ToImmutable();
  }

  public override string ToString()
  {
    StringBuilder sb = new();
    ToString(sb);
    return sb.ToString();
  }

  public void ToString(StringBuilder stringBuilder)
  {
    int argCount = Args.Count;
    int propertyCount = Properties.Count;

    stringBuilder.Append(DiagnosticId);
    if (Severity.HasValue)
    {
      stringBuilder.Append('@');
      stringBuilder.Append((int)Severity.Value);
    }
    stringBuilder.Append('|');
    stringBuilder.Append(FilePath);
    stringBuilder.Append('|');
    stringBuilder.Append(TextSpan.Start);
    stringBuilder.Append('|');
    stringBuilder.Append(TextSpan.Length);
    stringBuilder.Append('|');

    stringBuilder.Append(propertyCount);
    for (int i = 0; i < propertyCount; i++)
    {
      stringBuilder.Append('|');
      stringBuilder.Append(Properties[i]);
    }
    stringBuilder.Append('|');

    stringBuilder.Append(argCount);
    for (int i = 0; i < Args.Count; i++)
    {
      stringBuilder.Append('|');
      stringBuilder.Append(Args[i]);
    }
  }

  public static DiagnosticReference Create(string diagnosticId, Location location, params string[] args)
  {
    return new DiagnosticReference(diagnosticId, null, location.GetLineSpan().Path, location.SourceSpan, RecordList<string>.Empty, new RecordList<string>(args));
  }

  public static DiagnosticReference Create(string diagnosticId, Location location, string[] properties, params string[] args)
  {
    return new DiagnosticReference(diagnosticId, null, location.GetLineSpan().Path, location.SourceSpan, new RecordList<string>(properties), new RecordList<string>(args));
  }

  public static DiagnosticReference Create(string diagnosticId, DiagnosticSeverity? severity, Location location, params string[] args)
  {
    return new DiagnosticReference(diagnosticId, severity, location.GetLineSpan().Path, location.SourceSpan, RecordList<string>.Empty, new RecordList<string>(args));
  }

  public static DiagnosticReference Create(string diagnosticId, DiagnosticSeverity? severity, Location location, string[] properties, params string[] args)
  {
    return new DiagnosticReference(diagnosticId, severity, location.GetLineSpan().Path, location.SourceSpan, new RecordList<string>(properties), new RecordList<string>(args));
  }

  public static DiagnosticReference Parse(string text)
  {
    int cursor = 0;

    string diagnosticId = GetNextSegment(text, ref cursor);
    string filePath = GetNextSegment(text, ref cursor);
    int spanStart = int.Parse(GetNextSegment(text, ref cursor));
    int spanLength = int.Parse(GetNextSegment(text, ref cursor));

    int propertiesCount = text[cursor] - '0';
    cursor += 2;
    string[] properties = propertiesCount == 0
      ? Array.Empty<string>()
      : new string[propertiesCount];

    for (int i = 0; i < propertiesCount; i++)
    {
      properties[i] = GetNextSegment(text, ref cursor);
    }

    int argCount = text[cursor] - '0';
    cursor += 2;
    string[] args = argCount == 0
      ? Array.Empty<string>()
      : new string[argCount];

    for (int i = 0; i < argCount; i++)
    {
      args[i] = GetNextSegment(text, ref cursor);
    }

    TextSpan textSpan = new(spanStart, spanLength);

    DiagnosticSeverity? severity = null;
    int severityIndex = diagnosticId.IndexOf('@');
    if (severityIndex != -1)
    {
      severity = (DiagnosticSeverity)int.Parse(diagnosticId[(severityIndex + 1)..]);
      diagnosticId = diagnosticId[0..severityIndex];
    }

    return new DiagnosticReference(diagnosticId, severity, filePath, textSpan, new RecordList<string>(properties), new RecordList<string>(args));
    
    static string GetNextSegment(string text, ref int cursor)
    {
      int start = cursor;

      while (cursor != text.Length && text[cursor] is not '|')
      {
        cursor++;
      }

      string segment = text[start..cursor];
      cursor++;

      return segment;
    }
  }
}
