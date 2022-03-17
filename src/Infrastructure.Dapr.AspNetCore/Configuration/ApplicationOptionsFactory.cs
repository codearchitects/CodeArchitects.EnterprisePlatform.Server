using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration.Component;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

internal class ApplicationOptionsFactory : IApplicationOptionsFactory
{
  private readonly IDeserializer _deserializer;

  public ApplicationOptionsFactory()
  {
    _deserializer = new DeserializerBuilder()
      .IgnoreUnmatchedProperties()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();
  }

  public ApplicationOptions? FromFileProvider(IFileProvider provider)
  {
    IDirectoryContents contents = provider.GetDirectoryContents("/");
    if (!contents.Exists)
      return null;

    List<string> messageBusses = new List<string>();
    List<string> stateStores = new List<string>();

    foreach (IFileInfo file in contents)
    {
      if (!file.Exists || file.IsDirectory)
        continue;

      using Stream stream = file.CreateReadStream();
      using StreamReader reader = new StreamReader(stream);

      ComponentSchema? component = TryDeserialize(reader);
      if (component is null || !IsValid(component))
        continue;

      if (component.Spec!.Type!.StartsWith("state."))
      {
        stateStores.Add(component.Metadata!.Name!);
      }
      else if (component.Spec.Type.StartsWith("pubsub."))
      {
        messageBusses.Add(component.Metadata!.Name!);
      }
    }

    return new ApplicationOptions
    {
      MessageBusses = messageBusses,
      StateStores = stateStores
    };
  }

  private ComponentSchema? TryDeserialize(StreamReader reader)
  {
    try
    {
      return _deserializer.Deserialize<ComponentSchema>(reader);
    }
    catch
    {
      return null;
    }
  }

  private bool IsValid(ComponentSchema component)
  {
    return
      component.ApiVersion is not null &&
      component.Kind is "Component" or "component" &&
      component.Metadata is not null && IsValid(component.Metadata) &&
      component.Spec is not null && IsValid(component.Spec);
  }

  private bool IsValid(MetadataSchema metadata)
  {
    return metadata.Name is not null;
  }

  private bool IsValid(SpecSchema spec)
  {
    return spec.Type is not null;
  }
}
