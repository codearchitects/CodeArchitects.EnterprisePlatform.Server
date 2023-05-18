using CodeArchitects.Platform.Application.Identity;
using CodeArchitects.Platform.Common.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Application.DependencyInjection;

public class ApplicationServiceCollectionExtensionsTests
{
  [Fact]
  public void AddIdentityProfile_ShouldAddIdentityProfileAndHttpContextAccessor()
  {
    // Arrange
    ServiceCollection services = new();

    // Act
    services.AddIdentityProfile();

    // Assert
    services.Should().HaveCount(2)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IIdentityProfile<Guid>) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IHttpContextAccessor) && descriptor.Lifetime == ServiceLifetime.Singleton);
  }

  [Fact]
  public void AddIdentityProfileCustom_ShouldAddIdentityProfileAndHttpContextAccessor()
  {
    // Arrange
    ServiceCollection services = new();

    // Act
    services.AddIdentityProfile<IMyIdentityProfile, MyClaimsIdentityProfile>();

    // Assert
    services.Should().HaveCount(4)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IMyIdentityProfile) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IIdentityProfile<Guid>) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(ITenantProfile<Guid>) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IHttpContextAccessor) && descriptor.Lifetime == ServiceLifetime.Singleton);
  }

  private interface IMyIdentityProfile : IIdentityProfile<Guid>, ITenantProfile<Guid>
  {
    string MyClaim { get; }
  }

  private class MyClaimsIdentityProfile : ClaimsIdentityProfile<Guid>, IMyIdentityProfile
  {
    public MyClaimsIdentityProfile(IHttpContextAccessor httpContextAccessor)
      : base(httpContextAccessor)
    {
    }

    public string MyClaim => GetRequiredClaim("myClaimType");

    public Guid TenantId => Guid.Parse(GetRequiredClaim("tenantId"));
  }
}
