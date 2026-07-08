extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a foreign key database column in an entity model.
/// </summary>
[Experimental]
public interface IForeignKeyColumnModel : IColumnModel
{
  /// <summary>
  /// The index of this foreign key column within a composite foreign key.
  /// </summary>
  short ForeignKeyIndex { get; }

  /// <summary>
  /// The primary key column that this foreign key column references.
  /// </summary>
  IPrimaryKeyColumnModel PrimaryKeyColumn { get; }

  /// <summary>
  /// The navigation model associated with this foreign key column.
  /// </summary>
  INavigationModel Navigation { get; }
}
