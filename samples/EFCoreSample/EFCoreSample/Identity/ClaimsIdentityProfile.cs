using CodeArchitects.Platform.Application.Identity;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace EFCoreSample.Identity;

public class ClaimsIdentityProfile : ClaimsIdentityProfile<Guid>, IIdentityProfile
{
  public ClaimsIdentityProfile(IHttpContextAccessor httpContextAccessor)
    : base(httpContextAccessor)
  {
  }

  protected override string UserIdClaimType => ClaimTypes.NameIdentifier;

  public Guid TenantId => Guid.Parse(GetRequiredClaim(ClaimConstants.TenantId));
}
