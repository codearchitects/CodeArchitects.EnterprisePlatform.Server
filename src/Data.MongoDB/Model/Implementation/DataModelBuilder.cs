using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

internal sealed class DataModelBuilder
{
  private readonly Dictionary<Type, EntityModel> _entities = [];

  /// <summary>
  /// Determines whether a type can be mapped to a MongoDB collection.
  /// </summary>
  public bool IsEntityCandidate(Type type) =>
    type.IsClass && !type.IsAbstract && !type.IsGenericTypeDefinition &&
    (type.IsPublic || type.IsNestedPublic) &&
    (type.IsDefined(typeof(CollectionAttribute), inherit: false) ||
     type.IsDefined(typeof(TableAttribute), inherit: false));

  public DataModelBuilder AddEntity(Type type)
  {
    if (type is null)
      throw new ArgumentNullException(nameof(type));

    if (_entities.ContainsKey(type))
      return this;

    _entities.Add(type, EntityModel.Create(type));
    return this;
  }

  public DataModelBuilder AddEntitiesFrom(Assembly assembly, Func<Type, bool>? filter = null)
  {
    if (assembly is null)
      throw new ArgumentNullException(nameof(assembly));

    foreach (Type type in assembly.GetTypes())
    {
      if (IsEntityCandidate(type) && (filter?.Invoke(type) ?? true))
      {
        AddEntity(type);
      }
    }

    return this;
  }

  public DataModel Build()
  {
    if (_entities.Count == 0)
      throw new InvalidOperationException(
        "No MongoDB entity was registered. Use AddEntitiesFrom(assembly) or AddEntity<TEntity>(), " +
        "and make sure the entities are marked with [Collection] or [Table].");

    string[] collisions = [.. _entities.Values
      .GroupBy(entity => entity.CollectionName, StringComparer.Ordinal)
      .Where(group => group.Count() > 1)
      .Select(group => $"'{group.Key}' is mapped by {string.Join(", ", group.Select(entity => entity.Type.FullName))}")];

    if (collisions.Length > 0)
      throw new InvalidOperationException(
        $"Multiple entities are mapped to the same MongoDB collection: {string.Join("; ", collisions)}.");

    return new DataModel(_entities);
  }
}
