using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IForeignKeyModel
{
  bool IsComposite { get; }

  Type Type { get; }
  
  IReadOnlyCollection<IForeignKeyColumnModel> Columns { get; }
}
