using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents the primary key of an entity model.
/// </summary>
[Experimental]
public interface IPrimaryKeyModel<TKey> : IPrimaryKeyModel
  where TKey : IEquatable<TKey>
{
  /// <summary>
  /// A delegate that can be used to get the value of this primary key for a given object instance.
  /// </summary>
  new Getter<TKey> GetValue { get; }

  /// <summary>
  /// A delegate that can be used to set the value of this primary key for a given object instance.
  /// </summary>
  new Setter<TKey> SetValue { get; }

  /// <summary>
  /// Gets the component of the key which has the specified index.
  /// </summary>
  /// <param name="key">The instance of the primary key.</param>
  /// <param name="index">The index of the component.</param>
  /// <returns>The key component.</returns>
  object? GetKeyComponent(TKey key, int index);
}
