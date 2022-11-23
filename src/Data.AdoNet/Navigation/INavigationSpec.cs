using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

// internal interface INavigationPlan
// {
//   IEntityModel Entity { get; }
//   IEnumerable<IPropertyModel> Properties { get; }
//   IReadOnlyCollection<INavigationPlan> Navigations { get; }
// }

internal interface INavigationSpec
{
  string BuildSelectQuery();
}
