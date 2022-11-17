using CodeArchitects.Platform.Common.Identity;

namespace CodeArchitects.Platform.Data.Features.Multitenancy;

public class ProfileMultitenancyContext<TTenantId> : IMultitenancyContext
  where TTenantId : IEquatable<TTenantId>
{
  private readonly ITenantProfile<TTenantId> _profile;

  public ProfileMultitenancyContext(ITenantProfile<TTenantId> profile)
  {
    _profile = profile;
  }

  public virtual bool ShouldFilter => true;

  public object TenantId => _profile.TenantId;
}
