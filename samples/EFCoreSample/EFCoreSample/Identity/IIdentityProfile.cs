using CodeArchitects.Platform.Common.Identity;

namespace EFCoreSample.Identity;

public interface IIdentityProfile : IIdentityProfile<Guid>, ITenantProfile<Guid>
{
}
