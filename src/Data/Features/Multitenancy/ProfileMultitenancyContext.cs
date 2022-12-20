using CodeArchitects.Platform.Common.Identity;

namespace CodeArchitects.Platform.Data.Features.Multitenancy;

/// <summary>
/// Represents a multitenancy context that is based on a <see cref="ITenantProfile{TTenantId}"/>.
/// </summary>
/// <typeparam name="TTenantId">The type of the tenant identifier.</typeparam>
public class ProfileMultitenancyContext<TTenantId> : IMultitenancyContext
  where TTenantId : IEquatable<TTenantId>
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ProfileMultitenancyContext{TTenantId}"/> class.
  /// </summary>
  /// <param name="profile">The tenant profile to use as the basis for the multitenancy context.</param>
  public ProfileMultitenancyContext(ITenantProfile<TTenantId> profile)
  {
    Profile = profile;
  }

  /// <summary>
  /// The tenant profile used as the basis for the multitenancy context.
  /// </summary>
  protected ITenantProfile<TTenantId> Profile { get; }

  public virtual bool ShouldFilter => true;

  public object TenantId => Profile.TenantId;
}
