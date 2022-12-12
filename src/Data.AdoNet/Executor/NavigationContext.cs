using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

public readonly record struct NavigationContext(object Parent, INavigationModel NavigationModel);
