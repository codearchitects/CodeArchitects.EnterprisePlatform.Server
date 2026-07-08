extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an accessible simple navigation.
/// </summary>
/// <remarks>
/// A simple navigation is a navigation that does not require a junction table.
/// One-to-one associations and one-to-many associations are typically simple navigations.
/// </remarks>
[Experimental]
public interface IAccessibleSimpleNavigationModel : ISimpleNavigationModel, IAccessibleNavigationModel
{
}
