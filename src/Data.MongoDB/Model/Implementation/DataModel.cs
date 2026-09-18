using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

internal sealed class DataModel : IDataModel
{
  private readonly Dictionary<Type, EntityModel> _entities;

  public DataModel(IReadOnlyDictionary<Type, EntityModel> entities)
  {
    if (entities is null)
      throw new ArgumentNullException(nameof(entities));

    _entities = [with(entities.Count)];
    foreach (KeyValuePair<Type, EntityModel> entity in entities)
    {
      _entities.Add(entity.Key, entity.Value);
    }
  }

  public IReadOnlyCollection<IEntityModel> Entities => _entities.Values;

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
}
