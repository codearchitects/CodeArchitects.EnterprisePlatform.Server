using Microsoft.Extensions.FileProviders;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

public interface IDaprConfigurationBuilder
{
  IDaprConfiguration Configuration { get; }

  TSection AddSection<TSection>(string key) where TSection : class, new();
  void AddComponents(IFileProvider componentsFolder);
}
