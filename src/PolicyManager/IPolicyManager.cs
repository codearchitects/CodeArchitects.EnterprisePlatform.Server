using CodeArchitects.Platform.PolicyManager.Models;
using System.Security.Claims;

namespace CodeArchitects.Platform.PolicyManager;

/// <summary>
/// Interface for validating controllers authorization policies.
/// </summary>
public interface IPolicyManager
{
	/// <summary>
	/// Checks whether a specified controller resource can be accessed by a given user principal.
	/// </summary>
	/// <param name="policy">The resource policy name.</param>
	/// <param name="resource">The resource to be accessed.</param>
	/// <param name="principal">The principal representing the user attempting to access the resource.</param>
	/// <returns>True if the access is allowed, false otherwise.</returns>
	bool CheckAccess(string policy, string resource, ClaimsPrincipal principal);
}