extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a primary key database column in an entity model.
/// </summary>
[Experimental]
public interface IPrimaryKeyColumnModel : IAccessibleColumnModel
{
}
