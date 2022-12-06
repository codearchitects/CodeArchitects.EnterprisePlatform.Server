using CodeArchitects.Platform.Common.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class DataModel : IDataModel
{
  private readonly Dictionary<string, IEntityModel> _entities;

  public DataModel()
  {
    _entities = new();
  }

  public IReadOnlyCollection<IEntityModel> Entities => _entities.Values;

  public void AddEntity(IEntityModel entity)
  {
    _entities.Add(entity.Name, entity);
  }

  public bool TryGetEntity(string entityName, [NotNullWhen(true)] out IEntityModel? entity)
  {
    return _entities.TryGetValue(entityName, out entity);
  }

  public bool TryGetEntity<TEntity, TKey>(string entityName, [NotNullWhen(true)] out IEntityModel<TEntity, TKey>? entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_entities.TryGetValue(entityName, out IEntityModel? untypedEntity))
    {
      entity = null;
      return false;
    }

    Type keyType = untypedEntity.PrimaryKey.Type;
    if (keyType != typeof(TKey))
      throw new TypeArgumentException($"Wrong key type '{typeof(TKey).Name}' for entity '{untypedEntity.Type.Name}': expected '{keyType.Name}'.");

    entity = (IEntityModel<TEntity, TKey>)untypedEntity;
    return true;
  }
}
