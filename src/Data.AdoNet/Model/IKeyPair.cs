using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IKeyPair
{
  IPrimaryKeyColumnModel PrimaryKeyColumn { get; }
  
  IForeignKeyColumnModel ForeignKeyColumn { get; }

  IColumnModel FromColumn { get; }
  
  IColumnModel ToColumn { get; }
}