using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents a pair of primary key and foreign key columns in a navigation model.
/// </summary>
[Experimental]
public interface IKeyPair
{
  /// <summary>
  /// The primary key column.
  /// </summary>
  IPrimaryKeyColumnModel PrimaryKeyColumn { get; }

  /// <summary>
  /// The foreign key column.
  /// </summary>
  IColumnModel ForeignKeyColumn { get; }

  /// <summary>
  /// The column of the navigation's <see cref="INavigationModel.From"/> entity.
  /// </summary>
  IColumnModel FromColumn { get; }

  /// <summary>
  /// The column of the navigation's <see cref="INavigationModel.To"/> entity.
  /// </summary>
  IColumnModel ToColumn { get; }
}