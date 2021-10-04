using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeArchitects.Platform.Application.SignalR
{
  public static class ApplicationSignalRServerBuilderExtensions
  {
    public static ISignalRServerBuilder AddHubs(this ISignalRServerBuilder builder, params Assembly[] assemblies)
    {
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
