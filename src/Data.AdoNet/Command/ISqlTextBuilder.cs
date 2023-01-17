using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ISqlTextBuilder
{
  string BuildFindText(INavigationRoot root);
  string BuildInsertText(IEntityModel entityModel);
  string BuildUpdateText(IEntityModel entityModel);
  string BuildUpsertText(IEntityModel entityModel);
  string BuildRemoveText(IEntityModel entityModel);
  string BuildCountText(IEntityModel entityModel);
  string BuildCustomText(string query, INavigationRoot root);
}
