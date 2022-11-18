using CodeArchitects.Platform.Common.Identity;

namespace CodeArchitects.Platform.Data.Features.Multitenancy;

public class ProfileMultitenancyContext<TTenantId> : IMultitenancyContext
  where TTenantId : IEquatable<TTenantId>
{
  public ProfileMultitenancyContext(ITenantProfile<TTenantId> profile)
  {
    Profile = profile;
  }

  protected ITenantProfile<TTenantId> Profile { get; }

  public virtual bool ShouldFilter => true;

  public object TenantId => Profile.TenantId;
}
