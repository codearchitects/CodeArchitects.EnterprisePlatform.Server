using CodeArchitects.Platform.Application.Identity;
using CodeArchitects.Platform.Common.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Xunit;

namespace CodeArchitects.Platform.Application.DependencyInjection;

public class ApplicationServiceCollectionExtensionsTests
{
  [Fact]
  public void AddIdentityProfile_ShouldAddIdentityProfileAndHttpContextAccessor()
  {
    // Arrange
    ServiceCollection services = new ServiceCollection();

    // Act
    services.AddIdentityProfile();

    // Assert
    services.Should().HaveCount(3)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IIdentityProfile) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(ClaimsPrincipal) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IHttpContextAccessor) && descriptor.Lifetime == ServiceLifetime.Singleton);
  }

  [Fact]
  public void AddIdentityProfileCustom_ShouldAddIdentityProfileAndHttpContextAccessor()
  {
    // Arrange
    ServiceCollection services = new ServiceCollection();

    // Act
    services.AddIdentityProfile<IMyIdentityProfile, MyClaimsIdentityProfile>();

    // Assert
    services.Should().HaveCount(4)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IMyIdentityProfile) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IIdentityProfile) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(ClaimsPrincipal) && descriptor.Lifetime == ServiceLifetime.Scoped)
      .And.ContainSingle(descriptor => descriptor.ServiceType == typeof(IHttpContextAccessor) && descriptor.Lifetime == ServiceLifetime.Singleton);
  }

  private interface IMyIdentityProfile : IIdentityProfile
  {
    string MyClaim { get; }
  }

  private class MyClaimsIdentityProfile : ClaimsIdentityProfile, IMyIdentityProfile
  {
    public MyClaimsIdentityProfile(ClaimsPrincipal? claims)
      : base(claims)
    {
    }

    public string MyClaim => GetRequiredClaim("myClaimType");
  }
}
