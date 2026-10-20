using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System.Collections.Concurrent;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Serialization;

/// <summary>
/// Maps the key the provider resolved for an entity to the <c>_id</c> element.
/// </summary>
/// <remarks>
/// The driver only maps <c>Id</c>, <c>id</c> and <c>_id</c> on its own: without this convention a
/// <c>&lt;TypeName&gt;Id</c> key would be stored as an ordinary element, and every lookup by key,
/// which always targets <c>_id</c>, would miss. It runs with the other class-map conventions, before
/// the member and post-processing ones, so the key gets the same serializer, attributes and id
/// generator as a key called <c>Id</c>.
/// </remarks>
internal sealed class EntityKeyConvention : ConventionBase, IClassMapConvention
{
  // Keyed by the type that declares the key property: that is the class map the driver
  // requires the id member to be mapped on.
  private readonly ConcurrentDictionary<Type, string> _keys = new();

  /// <summary>
  /// Records the key member of an entity.
  /// </summary>
  /// <returns><c>false</c> when the declaring type already has a different key member.</returns>
  public bool TryAddKey(Type declaringType, string memberName) =>
    _keys.GetOrAdd(declaringType, memberName) == memberName;

  public void Apply(BsonClassMap classMap)
  {
    if (!_keys.TryGetValue(classMap.ClassType, out string? memberName))
      return;

    PropertyInfo? property = classMap.ClassType.GetProperty(
      memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

    if (property is not null)
    {
      classMap.MapIdMember(property);
    }
  }
}
