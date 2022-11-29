using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal interface ISqlTextBuilder
{
  string BuildSelectText(IEntityModel entity);
  string BuildSelectText(INavigationRoot root);
}
