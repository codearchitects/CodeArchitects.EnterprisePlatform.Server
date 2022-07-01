using CodeArchitects.Platform.Dapr.AspNetCore.Components;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

public interface IDaprConfiguration
{
  IReadOnlyList<ComponentSchema>? Components { get; }
  TSection? GetSection<TSection>() where TSection : class;
}
