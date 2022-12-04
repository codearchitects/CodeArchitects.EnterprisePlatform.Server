using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPrimaryKeyColumnModel : IColumnModel, IAccessibleColumnModel
{
  short PrimaryKeyIndex { get; }
}
