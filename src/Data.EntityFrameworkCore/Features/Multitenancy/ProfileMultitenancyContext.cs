using CodeArchitects.Platform.Common.Identity;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class ProfileMultitenancyContext<TTenantId> : Data.Features.Multitenancy.ProfileMultitenancyContext<TTenantId>
  where TTenantId : IEquatable<TTenantId>
{
  // This class exists only to put the ApplicationService attribute on the 'profile' parameter
  public ProfileMultitenancyContext([ApplicationService] ITenantProfile<TTenantId> profile)
    : base(profile)
  {
  }
}
