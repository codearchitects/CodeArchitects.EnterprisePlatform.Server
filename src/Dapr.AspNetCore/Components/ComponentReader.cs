using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

internal class ComponentReader : IComponentReader
{
  private readonly IDeserializer _deserializer;

  public ComponentReader()
  {
    _deserializer = new DeserializerBuilder()
      .IgnoreUnmatchedProperties()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();
  }

  public ComponentSchema FromFile(IFileInfo file)
  {
    using Stream stream = file.CreateReadStream();
    using StreamReader reader = new StreamReader(stream);

    ComponentSchema component = _deserializer.Deserialize<ComponentSchema>(reader);
    Validate(component);

    return component;
  }

  private static void Validate(ComponentSchema component)
  {
    ValidationContext context = new ValidationContext(component);
    Validator.ValidateObject(component, context);
  }
}
