using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IForeignKeyColumnModel : IColumnModel
{
  short ForeignKeyIndex { get; }

  IPrimaryKeyColumnModel PrimaryKeyColumn { get; }

  INavigationModel Navigation { get; }
}
