using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents the primary key of an entity model.
/// </summary>
[Experimental]
public interface IPrimaryKeyModel
{
  /// <summary>
  /// Indicates whether this primary key is a composite key (made up of multiple columns).
  /// </summary>
  bool IsComposite { get; }

  /// <summary>
  /// The type of the primary key.
  /// </summary>
  Type Type { get; }

  /// <summary>
  /// The columns that make up this primary key.
  /// </summary>
  IReadOnlyList<IPrimaryKeyColumnModel> Columns { get; }

  /// <summary>
  /// A delegate that can be used to get the value of this primary key for a given object instance.
  /// </summary>
  Getter<object?> GetValue { get; }

  /// <summary>
  /// A delegate that can be used to set the value of this primary key for a given object instance.
  /// </summary>
  Setter<object?> SetValue { get; }

  /// <summary>
  /// Tries to get the primary key column with the given name.
  /// </summary>
  /// <param name="name">The name of the column to get.</param>
  /// <param name="column">The column, if found, <c>null</c> otherwise.</param>
  /// <returns><c>true</c> if the column was found, <c>false</c> otherwise.</returns>
  bool TryGetColumn(ReadOnlySpan<char> name, [NotNullWhen(true)] out IPrimaryKeyColumnModel? column);
}
