using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPrimaryKeyModel
{
  bool IsComposite { get; }
  
  Type Type { get; }
  
  IReadOnlyList<IPrimaryKeyColumnModel> Columns { get; }

  Getter<object?> GetValue { get; }

  Setter<object?> SetValue { get; }
}
