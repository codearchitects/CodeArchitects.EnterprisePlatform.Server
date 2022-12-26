using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ISqlTextCache
{
  bool TryGetFindText(INavigationRoot root, [NotNullWhen(true)] out string? text);
  void AddFindText(INavigationRoot root, string text);

  bool TryGetInsertText(IEntityModel entityModel, [NotNullWhen(true)] out string? text);
  void AddInsertText(IEntityModel entityModel, string text);

  bool TryGetUpdateText(IEntityModel entityModel, [NotNullWhen(true)] out string? text);
  void AddUpdateText(IEntityModel entityModel, string text);

  bool TryGetUpsertText(IEntityModel entityModel, [NotNullWhen(true)] out string? text);
  void AddUpsertText(IEntityModel entityModel, string text);

  bool TryGetRemoveText(IEntityModel entityModel, [NotNullWhen(true)] out string? text);
  void AddRemoveText(IEntityModel entityModel, string text);
}
