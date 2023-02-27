using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Utils;

internal readonly record struct DiagnosticResult<T>(T Value, RecordList<Diagnostic> Diagnostics)
  where T : IEquatable<T>;
