using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal class SqlTextCache : ISqlTextCache
{
  private readonly IDictionary<NavigationSpec, string> _selectTexts;
  private readonly IDictionary<IEntityModel, string> _insertTexts;
  private readonly IDictionary<IEntityModel, string> _updateTexts;
  private readonly IDictionary<IEntityModel, string> _upsertTexts;
  private readonly IDictionary<IEntityModel, string> _deleteTexts;

  public SqlTextCache(
    IDictionary<NavigationSpec, string> selectTexts,
    IDictionary<IEntityModel, string> insertTexts,
    IDictionary<IEntityModel, string> updateTexts,
    IDictionary<IEntityModel, string> upsertTexts,
    IDictionary<IEntityModel, string> deleteTexts)
  {
    _selectTexts = selectTexts;
    _insertTexts = insertTexts;
    _updateTexts = updateTexts;
    _upsertTexts = upsertTexts;
    _deleteTexts = deleteTexts;
  }

  public bool TryGetFindText(in NavigationSpec spec, [NotNullWhen(true)] out string? text)
  {
    return _selectTexts.TryGetValue(spec, out text);
  }

  public void AddFindText(in NavigationSpec spec, string text)
  {
    _selectTexts.Add(spec, text);
  }

  public bool TryGetInsertText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    return _insertTexts.TryGetValue(entityModel, out text);
  }

  public void AddInsertText(IEntityModel entityModel, string text)
  {
    _insertTexts.Add(entityModel, text);
  }

  public bool TryGetUpdateText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    return _updateTexts.TryGetValue(entityModel, out text);
  }

  public void AddUpdateText(IEntityModel entityModel, string text)
  {
    _updateTexts.Add(entityModel, text);
  }

  public bool TryGetUpsertText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    throw new NotImplementedException();
  }

  public void AddUpsertText(IEntityModel entityModel, string text)
  {
    throw new NotImplementedException();
  }

  public bool TryGetRemoveText(IEntityModel entityModel, [NotNullWhen(true)] out string? text)
  {
    return _deleteTexts.TryGetValue(entityModel, out text);
  }

  public void AddRemoveText(IEntityModel entityModel, string text)
  {
    _deleteTexts.Add(entityModel, text);
  }

  public static SqlTextCache Create()
  {
    return new(
      new ConcurrentDictionary<NavigationSpec, string>(),
      new ConcurrentDictionary<IEntityModel, string>(),
      new ConcurrentDictionary<IEntityModel, string>(),
      new ConcurrentDictionary<IEntityModel, string>(),
      new ConcurrentDictionary<IEntityModel, string>());
  }
}
