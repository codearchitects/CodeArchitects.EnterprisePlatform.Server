using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

internal static class ComponentSchemaExtensions
{
  public static IEnumerable<string> GetComponentNames(this IEnumerable<ComponentSchema> components, string componentType)
  {
    return components
      .Where(c => c.Spec.Type.StartsWith(componentType))
      .Select(c => c.Metadata.Name);
  }
}
