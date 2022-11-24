using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

internal interface ISqlTextBuilder
{
  string BuildSelectText(INavigationRoot spec);
}
