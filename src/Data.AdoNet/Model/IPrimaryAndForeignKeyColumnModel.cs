using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a database column in an entity model that serves both as a primary key and a foreign key.
/// </summary>
[Experimental]
public interface IPrimaryAndForeignKeyColumnModel : IPrimaryKeyColumnModel, IForeignKeyColumnModel
{
}
