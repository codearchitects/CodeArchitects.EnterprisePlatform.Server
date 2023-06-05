using CodeArchitects.Platform.Application.Identity;
using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Identity;
using CodeArchitects.Platform.Common.Utils;
using System.Reflection;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions method for <see cref="IServiceCollection"/>.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
  /// <summary>
  /// Adds a basic identity profile to the services.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddIdentityProfile(this IServiceCollection services)
  {
    return services
      .AddHttpContextAccessor()
      .AddScoped<IIdentityProfile<Guid>, ClaimsIdentityProfile<Guid>>();
  }

  /// <summary>
  /// Adds a basic identity profile to the services.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddIdentityProfile<TUserId>(this IServiceCollection services)
    where TUserId : IEquatable<TUserId>
  {
    if (typeof(TUserId) != typeof(string))
    {
      Parsable.EnsureInitialized(typeof(TUserId));
    }

    return services
      .AddHttpContextAccessor()
      .AddScoped<IIdentityProfile<TUserId>, ClaimsIdentityProfile<TUserId>>();
  }

  /// <summary>
  /// Adds a custom identity profile to the services.
  /// </summary>
  /// <typeparam name="TInterface">The identity profile interface type.</typeparam>
  /// <typeparam name="TImplementation">The identity profile implementation type.</typeparam>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  /// <exception cref="InvalidOperationException">Thrown when <typeparamref name="TImplementation"/> does not have a constructor which accepts a single parameter of type <see cref="ClaimsPrincipal"/>.</exception>
  public static IServiceCollection AddIdentityProfile<TInterface, TImplementation>(this IServiceCollection services)
    where TInterface : class
    where TImplementation : class, TInterface
  {
    Type interfaceType = typeof(TInterface);
    Type identityProfileType;
    Type? tenantProfileType;
    
    try
    {
      identityProfileType = interfaceType.GetGenericInterfaces(typeof(IIdentityProfile<>)).Single();
    }
    catch (InvalidOperationException)
    {
      throw new TypeArgumentException($"'{nameof(TInterface)}' must implement '{nameof(IIdentityProfile<string>)}' exactly once.", nameof(TInterface)); // 'string' type parameter only to make 'nameof' happy
    }

    try
    {
      tenantProfileType = interfaceType.GetGenericInterfaces(typeof(ITenantProfile<>)).SingleOrDefault();
    }
    catch (InvalidOperationException)
    {
      throw new TypeArgumentException($"'{nameof(TInterface)}' must implement '{nameof(ITenantProfile<string>)}' at most once.", nameof(TInterface)); // 'string' type parameter only to make 'nameof' happy
    }

    Type userIdType = identityProfileType.GetGenericArguments()[0];
    if (userIdType != typeof(string))
    {
      Parsable.EnsureInitialized(userIdType);
    }

    services
      .AddHttpContextAccessor()
      .AddScoped<TInterface, TImplementation>()
      .AddScoped(identityProfileType, sp => sp.GetRequiredService<TInterface>());

    if (tenantProfileType is not null)
    {
      services.AddScoped(tenantProfileType, sp => sp.GetRequiredService<TInterface>());
    }

    return services;
  }
}
