using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPrimaryKeyModel
{
  bool IsComposite { get; }
  
  Type Type { get; }
  
  IReadOnlyList<IPrimaryKeyColumnModel> Columns { get; }

  Getter<object?> GetValue { get; }

  Setter<object?> SetValue { get; }

  bool TryGetColumn(ReadOnlySpan<char> name, [NotNullWhen(true)] out IPrimaryKeyColumnModel? column);
}
