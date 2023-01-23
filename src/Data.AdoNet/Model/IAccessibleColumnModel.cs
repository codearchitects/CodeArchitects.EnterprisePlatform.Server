using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a database column in an entity model whose member is accessible.
/// </summary>
[Experimental]
public interface IAccessibleColumnModel : IColumnModel, IAccessibleMemberModel
{
}
