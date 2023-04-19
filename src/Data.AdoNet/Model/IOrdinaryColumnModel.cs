using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an ordinary database column in an entity model, which is not a primary key or a foreign key.
/// </summary>
[Experimental]
public interface IOrdinaryColumnModel : IAccessibleColumnModel
{
}
