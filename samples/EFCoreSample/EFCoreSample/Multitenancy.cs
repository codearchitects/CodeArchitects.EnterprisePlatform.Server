using CodeArchitects.Platform.Common.Identity;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using CodeArchitects.Platform.Data.Features.Multitenancy;

namespace EFCoreSample;

public class MultitenancyContext : ProfileMultitenancyContext<Guid>
{
  public MultitenancyContext([ApplicationService] ITenantProfile<Guid> profile)
    : base(profile)
  {
  }

  public override bool ShouldFilter => Profile.TenantId != Guid.Parse("8639353b-9cb7-4941-b581-f9e093e45959");
}

public class MultitenancyDescriptor : DefaultMultitenancyDescriptor<Guid>
{
  public override Type? MultitenancyContextType => typeof(MultitenancyContext);
}
