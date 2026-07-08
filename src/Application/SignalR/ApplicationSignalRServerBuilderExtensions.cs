using CodeArchitects.Platform.Application.SignalR;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="ISignalRServerBuilder"/>.
/// </summary>
public static class ApplicationSignalRServerBuilderExtensions
{
  /// <summary>
  /// Registers with transient lifetime all strongly-typed hubs defined in the provided assemblies.
  /// </summary>
  /// <param name="builder">The builder instance.</param>
  /// <param name="assemblies">The assemblies to scan for handlers. If empty, the calling assembly will be used.</param>
  /// <returns>The same builder.</returns>
  public static ISignalRServerBuilder AddHubs(this ISignalRServerBuilder builder, params Assembly[] assemblies)
  {
    if (assemblies.Length == 0)
    {
      assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
    }
    HubConfiguration configuration = new(assemblies);
    builder.Services.AddSingleton(configuration);
    foreach (KeyValuePair<Type, Type> entry in configuration.HubMap)
    {
      builder.Services.AddTransient(entry.Key, entry.Value);
    }
    return builder;
  }
}
