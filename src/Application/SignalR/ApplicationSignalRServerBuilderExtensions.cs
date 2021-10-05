using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeArchitects.Platform.Application.SignalR
{
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
      HubConfiguration configuration = new HubConfiguration(assemblies);
      builder.Services.AddSingleton(configuration);
      foreach (KeyValuePair<Type, Type> entry in configuration.HubMap)
      {
        builder.Services.AddTransient(entry.Key, entry.Value);
      }
      return builder;
    }
  }
}
