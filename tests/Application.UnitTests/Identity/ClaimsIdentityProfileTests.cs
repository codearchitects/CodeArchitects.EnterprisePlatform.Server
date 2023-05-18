using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
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
    ClaimsPrincipal principal = Mock.Of<ClaimsPrincipal>(instance => instance.Identity!.IsAuthenticated == expected, MockBehavior.Strict);
    IHttpContextAccessor httpContextAccessor = Mock.Of<IHttpContextAccessor>(instance => instance.HttpContext!.User == principal, MockBehavior.Strict);

    ClaimsIdentityProfile<Guid> sut = new(httpContextAccessor);

    // Act
    bool actual = sut.IsAuthenticated;

    // Assert
    actual.Should().Be(expected);
  }

  [Fact]
  public void IsAuthenticated_ShouldBeFalse_WhenClaimsPrincipalHasNoIdentities()
  {
    // Arrange
    ClaimsPrincipal principal = new(Enumerable.Empty<ClaimsIdentity>());
    IHttpContextAccessor httpContextAccessor = Mock.Of<IHttpContextAccessor>(instance => instance.HttpContext!.User == principal, MockBehavior.Strict);

    ClaimsIdentityProfile<Guid> sut = new(httpContextAccessor);

    // Act
    bool isAuthenticated = sut.IsAuthenticated;

    // Assert
    isAuthenticated.Should().BeFalse();
  }

  [Fact]
  public void UserId_ShouldBe_ValueOfNameIdentifierClaim()
  {
    // Arrange
    Claim userIdClaim = new(JwtRegisteredClaimNames.Sub, "c88e015e-b550-4fd4-83ab-65cf9495f3c2");
    ClaimsPrincipal principal = Mock.Of<ClaimsPrincipal>(instance =>
      instance.Identity!.IsAuthenticated == true &&
      instance.FindFirst(JwtRegisteredClaimNames.Sub) == userIdClaim,
      MockBehavior.Strict);
    IHttpContextAccessor httpContextAccessor = Mock.Of<IHttpContextAccessor>(instance => instance.HttpContext!.User == principal, MockBehavior.Strict);

    ClaimsIdentityProfile<Guid> sut = new(httpContextAccessor);

    // Act
    Guid userId = sut.UserId;

    // Assert
    userId.Should().Be(userIdClaim.Value);
  }
}
