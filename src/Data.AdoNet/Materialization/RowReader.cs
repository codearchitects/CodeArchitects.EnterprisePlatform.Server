using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal delegate object KeyReader(IDataReader reader, int offset);
internal delegate object EntityReader(IDataReader reader, int offset);

internal class RowReader : IRowReader
{
  private readonly KeyReader _keyReader;
  private readonly EntityReader _entityReader;
  private readonly IEntityModel _model;
  private readonly int _keyIndex;

  public RowReader(KeyReader keyReader, EntityReader entityReader, IEntityModel model, int keyIndex)
  {
    _keyReader = keyReader;
    _entityReader = entityReader;
    _model = model;
    _keyIndex = keyIndex;
  }

  public object? ReadRow(IDataReader reader, ref int offset, IMaterializerHub hub, IReadOnlyCollection<INavigation> navigations)
  {
    if (reader.IsDBNull(_keyIndex + offset))
      return null;

    object key = _keyReader(reader, offset);
    IdentityCacheKey cacheKey = new(_model, key);

    if (!hub.TryGetExisting(cacheKey, out object? entity))
    {
      entity = _entityReader(reader, offset);
      hub.AddMaterialized(cacheKey, entity);
    }

    offset += _model.Properties.Count;

    foreach (INavigation navigation in navigations)
    {
      IAccessibleNavigationModel navigationModel = navigation.Model;

      object? navigationEntity = hub.ReadRow(reader, ref offset, navigation.Target, navigation.Children);
      if (navigationEntity is null)
        continue;

      if (navigation.Model.IsCollection)
      {
        IIdentityCollection collection;
        if (navigationModel.GetValue(entity) is { } property)
        {
          collection = (IIdentityCollection)property;
        }
        else
        {
          collection = hub.CreateCollection(navigationModel);
          navigationModel.SetValue(entity, collection);
        }

        collection.AddEntity(navigationEntity);
        continue;
      }

      navigationModel.SetValue(entity, navigationEntity);
    }

    return entity;
  }
}