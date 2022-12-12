using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ISqlTextBuilder
{
  string BuildSelectText(NavigationSpec spec);
  string BuildInsertText(IEntityModel entity);
  string BuildUpdateText(IEntityModel entity);
  string BuildDeleteText(IEntityModel entity);
}
