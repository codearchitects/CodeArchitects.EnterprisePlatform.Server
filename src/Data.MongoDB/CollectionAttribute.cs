namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Specifies the name of the collection in MongoDB for a given class.
/// </summary>
/// <param name="name">The name of the collection in MongoDB.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CollectionAttribute(string name) : Attribute
{
  /// <summary>
  /// Gets the name of the collection in MongoDB.
  /// </summary>
  public string Name { get; } = name;
}
