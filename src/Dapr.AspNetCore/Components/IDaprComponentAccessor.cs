using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

/// <summary>
/// Retrieves component schemas.
/// </summary>
public interface IDaprComponentAccessor
{
  /// <summary>
  /// The list of registered components.
  /// </summary>
  IReadOnlyList<ComponentSchema> Components { get; }
}
