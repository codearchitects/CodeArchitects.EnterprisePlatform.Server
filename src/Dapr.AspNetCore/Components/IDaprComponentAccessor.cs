using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

public interface IDaprComponentAccessor
{
  IReadOnlyList<ComponentSchema> Components { get; }
}
