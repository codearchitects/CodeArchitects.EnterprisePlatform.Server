using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IKeyPair
{
  IColumnModel PrimaryKey { get; }
  
  IColumnModel ForeignKey { get; }

  IColumnModel FromColumn { get; }
  
  IColumnModel ToColumn { get; }
}