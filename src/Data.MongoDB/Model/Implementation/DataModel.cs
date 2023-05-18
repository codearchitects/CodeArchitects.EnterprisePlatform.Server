using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.MongoDB.Model.Implementation;

internal class DataModel : IDataModel
{
  private readonly Dictionary<Type, EntityModel> _entities;

  public DataModel()
  {
    _entities = new();
  }

  public IReadOnlyCollection<IEntityModel> Entities => _entities.Values;

  public void AddEntity(Type entityType)
  {
    EntityModel entity = EntityModel.Create(entityType);

    _entities.Add(entity.Type, entity);
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
}
