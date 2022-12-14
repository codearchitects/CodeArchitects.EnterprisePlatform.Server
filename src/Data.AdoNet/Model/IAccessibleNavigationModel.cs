using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IAccessibleNavigationModel : INavigationModel, IAccessibleMemberModel
{
  [MemberNotNullWhen(true, nameof(CollectionAccessor))]
  new bool IsCollection { get; }

  new ICollectionAccessor? CollectionAccessor { get; }
}
