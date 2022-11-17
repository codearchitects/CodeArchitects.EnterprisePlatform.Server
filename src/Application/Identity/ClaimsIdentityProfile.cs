using CodeArchitects.Platform.Common.Identity;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace CodeArchitects.Platform.Application.Identity;

/// <summary>
/// Implementation of <see cref="IIdentityProfile{TUserId, TTenantId}"/> based on the current user's claims.
/// </summary>
public class ClaimsIdentityProfile : IIdentityProfile<Guid, Guid>
{
  private Guid? _userId;
  private Guid? _tenantId;

  public ClaimsIdentityProfile(ClaimsPrincipal? claims)
  {
    Claims = claims;
  }

  /// <summary>
  /// The user's claims.
  /// </summary>
  protected ClaimsPrincipal? Claims { get; }

  /// <summary>
  /// The claim type of the user id claim.
  /// </summary>
  public virtual string UserIdClaimType => ClaimTypes.NameIdentifier;

  /// <summary>
  /// The claim type of the tenant id claim.
  /// </summary>
  public virtual string TenantIdClaimType => ClaimConstants.TenantId;

  public bool IsAuthenticated => Claims?.Identity?.IsAuthenticated ?? false;

  public Guid UserId => _userId ??= Guid.Parse(GetRequiredClaim(UserIdClaimType));

  public Guid TenantId => _tenantId ??= Guid.Parse(GetRequiredClaim(TenantIdClaimType));

  protected string? GetClaim(string claimType)
  {
    if (Claims is null)
      return null;

    return Claims.FindFirstValue(claimType);
  }

  protected string GetRequiredClaim(string claimType)
  {
    return GetClaim(claimType) ?? throw new AuthenticationException();
  }
}
