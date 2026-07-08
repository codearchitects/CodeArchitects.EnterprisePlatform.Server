using Microsoft.AspNetCore.SignalR;
using System.Reflection;

namespace CodeArchitects.Platform.Application.SignalR;

internal class HubConfiguration
{
  public HubConfiguration(Assembly[] assemblies)
  {
    HubMap = assemblies
      .Distinct()
      .SelectMany(x => x.GetTypes())
      .Distinct()
      .Where(x => x.BaseType is not null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(Hub<>))
      .ToDictionary(GetHubInterface);
  }

  public IReadOnlyDictionary<Type, Type> HubMap { get; }

  private static Type GetHubInterface(Type hubType)
  {
    return hubType.BaseType!.GenericTypeArguments[0];
  }
}
