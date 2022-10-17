using Microsoft.Identity.Web;
using System.Security.Claims;

namespace CodeArchitects.Platform.Application.Identity;

public class ClaimsIdentityProfileTests
{
  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void IsAuthenticated_ShouldBe_PrincipalIdentityIsAuthenticated(bool expected)
  {
    // Arrange
    ClaimsPrincipal principal = Mock.Of<ClaimsPrincipal>(principal => principal.Identity!.IsAuthenticated == expected, MockBehavior.Strict);

    ClaimsIdentityProfile sut = new ClaimsIdentityProfile(principal);

    // Act
    bool actual = sut.IsAuthenticated;

    // Assert
    actual.Should().Be(expected);
  }

  [Fact]
  public void IsAuthenticated_ShouldBeFalse_WhenClaimsPrincipalHasNoIdentities()
  {
    // Arrange
    ClaimsPrincipal principal = new ClaimsPrincipal(Enumerable.Empty<ClaimsIdentity>());

    ClaimsIdentityProfile sut = new ClaimsIdentityProfile(principal);

    // Act
    bool isAuthenticated = sut.IsAuthenticated;

    // Assert
    isAuthenticated.Should().BeFalse();
  }

  [Fact]
  public void UserId_ShouldBe_ValueOfNameIdentifierClaim()
  {
    // Arrange
    Claim userIdClaim = new Claim(ClaimTypes.NameIdentifier, "c88e015e-b550-4fd4-83ab-65cf9495f3c2");
    ClaimsPrincipal principal = Mock.Of<ClaimsPrincipal>(principal => principal.FindFirst(ClaimTypes.NameIdentifier) == userIdClaim, MockBehavior.Strict);

    ClaimsIdentityProfile sut = new ClaimsIdentityProfile(principal);

    // Act
    Guid userId = sut.UserId;

    // Assert
    userId.Should().Be(userIdClaim.Value);
  }

  [Fact]
  public void TenantId_ShouldBe_ValueOfTenantIdClaim()
  {
    // Arrange
    Claim tenantIdClaim = new Claim(ClaimConstants.TenantId, "c88e015e-b550-4fd4-83ab-65cf9495f3c2");
    ClaimsPrincipal principal = Mock.Of<ClaimsPrincipal>(principal => principal.FindFirst(ClaimConstants.TenantId) == tenantIdClaim, MockBehavior.Strict);

    ClaimsIdentityProfile sut = new ClaimsIdentityProfile(principal);

    // Act
    Guid tenantId = sut.TenantId;

    // Assert
    tenantId.Should().Be(tenantIdClaim.Value);
  }
}
