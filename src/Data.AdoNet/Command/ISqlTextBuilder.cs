using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ISqlTextBuilder
{
  string BuildFindText(NavigationSpec spec);
  string BuildInsertText(IEntityModel entityModel);
  string BuildUpdateText(IEntityModel entityModel);
  string BuildUpsertText(IEntityModel entityModel);
  string BuildRemoveText(IEntityModel entityModel);
}
