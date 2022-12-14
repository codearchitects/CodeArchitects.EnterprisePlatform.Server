using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IAccessibleSkipNavigationModel : ISkipNavigationModel, IAccessibleNavigationModel
{
  new ICollectionAccessor CollectionAccessor { get; }
}
