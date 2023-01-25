using CodeArchitects.Platform.Application.Identity;
using CodeArchitects.Platform.Common.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions method for <see cref="IServiceCollection"/>.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
  /// <summary>
  /// Adds a base identity profile to the services.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddIdentityProfile(this IServiceCollection services)
  {
    return services
      .AddHttpContextAccessor()
      .AddScoped(delegate (IServiceProvider services)
      {
        IHttpContextAccessor httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
        HttpContext? context = httpContextAccessor.HttpContext;

        return context?.User;
      })
      .AddScoped<IIdentityProfile<Guid, Guid>, ClaimsIdentityProfile>()
      .AddScoped<IUserProfile<Guid>>(sp => sp.GetRequiredService<IIdentityProfile<Guid, Guid>>())
      .AddScoped<ITenantProfile<Guid>>(sp => sp.GetRequiredService<IIdentityProfile<Guid, Guid>>());
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
    where TInterface : class, IIdentityProfile<Guid, Guid>
    where TImplementation : class, TInterface
  {
    return services
      .AddHttpContextAccessor()
      .AddScoped(delegate (IServiceProvider services)
      {
        IHttpContextAccessor httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
        HttpContext? context = httpContextAccessor.HttpContext;

        return context?.User;
      })
      .AddScoped<TInterface, TImplementation>()
      .AddScoped<IIdentityProfile<Guid, Guid>>(sp => sp.GetRequiredService<TInterface>())
      .AddScoped<IUserProfile<Guid>>(sp => sp.GetRequiredService<IIdentityProfile<Guid, Guid>>())
      .AddScoped<ITenantProfile<Guid>>(sp => sp.GetRequiredService<IIdentityProfile<Guid, Guid>>());
  }
}
