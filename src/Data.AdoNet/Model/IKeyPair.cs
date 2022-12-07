using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IKeyPair
{
  IPrimaryKeyColumnModel PrimaryKey { get; }
  
  IForeignKeyColumnModel ForeignKey { get; }

  IColumnModel FromColumn { get; }
  
  IColumnModel ToColumn { get; }
}