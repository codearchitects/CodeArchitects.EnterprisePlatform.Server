extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class DataModel : IDataModel
{
  private readonly Dictionary<Type, EntityModel> _entities;

  public DataModel()
  {
    _entities = new();
  }

  public IReadOnlyCollection<EntityModel> Entities => _entities.Values;

  IReadOnlyCollection<IEntityModel> IDataModel.Entities => Entities;

  public void AddEntity(EntityModel entity)
  {
    _entities.Add(entity.Type, entity);
  }

  public bool TryGetEntity(Type entityType, [NotNullWhen(true)] out EntityModel? entity)
  {
    return _entities.TryGetValue(entityType, out entity);
  }

  public bool TryGetEntity(Type entityType, [NotNullWhen(true)] out IEntityModel? entity)
  {
    if (_entities.TryGetValue(entityType, out EntityModel? entityModel))
    {
      entity = entityModel;
      return true;
    }

    entity = null;
    return false;
  }

  public bool TryGetEntity<TEntity, TKey>([NotNullWhen(true)] out IEntityModel<TEntity, TKey>? entity)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (!_entities.TryGetValue(typeof(TEntity), out EntityModel? untypedEntity))
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
