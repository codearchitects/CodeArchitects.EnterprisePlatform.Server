using CodeArchitects.Platform.Common.Identity;
using CodeArchitects.Platform.Common.Utils;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CodeArchitects.Platform.Application.Identity;

/// <summary>
/// Implementation of <see cref="IIdentityProfile{TUserId}"/> based on the current user's claims.
/// </summary>
public class ClaimsIdentityProfile<TUserId> : IIdentityProfile<TUserId>
  where TUserId : IEquatable<TUserId>
{
  private static readonly Parse<TUserId> s_parseUserId = typeof(TUserId) == typeof(string)
    ? (Parse<TUserId>)(Delegate)(new Parse<string>(s => s))
    : Parsable<TUserId>.Parse;

  /// <summary>
  /// Creates a new <see cref="ClaimsIdentityProfile{TUserId}"/> instance.
  /// </summary>
  /// <param name="httpContextAccessor">The accessor of the <see cref="HttpContext"/>.</param>
  public ClaimsIdentityProfile(IHttpContextAccessor httpContextAccessor)
  {
    Claims = httpContextAccessor.HttpContext?.User;
  }

  /// <summary>
  /// The user's claims.
  /// </summary>
  protected ClaimsPrincipal? Claims { get; }

  /// <summary>
  /// Returns <see langword="true"/> if the user is authenticated, <see langword="false"/> otherwise.
  /// </summary>
  [MemberNotNullWhen(true, nameof(Claims))]
  public bool IsAuthenticated => Claims?.Identity?.IsAuthenticated ?? false;

  /// <summary>
  /// The type of the user id claim. Defaults to <see cref="JwtRegisteredClaimNames.Sub"/>.
  /// </summary>
  protected virtual string UserIdClaimType => JwtRegisteredClaimNames.Sub;

  /// <summary>
  /// The id of the current user.
  /// </summary>
  public TUserId UserId
  {
    get
    {
      string userIdClaim = GetRequiredClaim(UserIdClaimType);
      try
      {
        return s_parseUserId(userIdClaim);
      }
      catch (FormatException)
      {
        throw new AuthenticationException();
      }
    }
  }

  /// <summary>
  /// Returns a claim value corresponding to <paramref name="claimType"/>, if present, or <see langword="null"/> otherwise.
  /// </summary>
  /// <param name="claimType">The key of the claim to retrieve.</param>
  /// <returns>The claim value.</returns>
  /// <exception cref="AuthenticationException">Thrown when the user is not authenticated.</exception>
  protected string? GetClaim(string claimType)
  {
    if (!IsAuthenticated)
      throw new AuthenticationException();

    return Claims.FindFirstValue(claimType);
  }

  /// <summary>
  /// Returns a claim value corresponding to <paramref name="claimType"/>, if present, or <see langword="null"/> otherwise it throws.
  /// </summary>
  /// <param name="claimType">The key of the claim to retrieve.</param>
  /// <returns>The claim value.</returns>
  /// <exception cref="AuthenticationException">Thrown when the user is not authenticated or the claim is not found.</exception>
  protected string GetRequiredClaim(string claimType)
  {
    return GetClaim(claimType) ?? throw new AuthenticationException();
  }
}
