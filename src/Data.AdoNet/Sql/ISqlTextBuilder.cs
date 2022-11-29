using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal interface ISqlTextBuilder
{
  string BuildSelectText(NavigationSpec spec);
}
