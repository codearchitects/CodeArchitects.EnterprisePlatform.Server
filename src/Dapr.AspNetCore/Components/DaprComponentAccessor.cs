using CodeArchitects.Platform.Dapr.AspNetCore.Components.Schema;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Components;

internal class DaprComponentAccessor : IDaprComponentAccessor
{
  private readonly IComponentReader _reader;
  private readonly ILogger _logger;
  private readonly List<ComponentSchema> _components;

  public DaprComponentAccessor(IComponentReader reader, ILogger logger, List<ComponentSchema> components)
  {
    _reader = reader;
    _logger = logger;
    _components = components;
  }

  public IReadOnlyList<ComponentSchema> Components => _components;

  public void AddComponents(IFileProvider componentsFolder)
  {
    IDirectoryContents contents = componentsFolder.GetDirectoryContents("/");
    if (!contents.Exists)
      return;

    foreach (IFileInfo file in contents)
    {
      if (!file.Exists || file.IsDirectory)
        continue;

      ComponentSchema component;
      try
      {
        component = _reader.FromFile(file);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Could not read component in file {0}", file.PhysicalPath ?? file.Name);
        continue;
      }

      _components.Add(component);
    }
  }

  public static DaprComponentAccessor Create(IComponentReader reader, ILogger logger)
  {
    return new DaprComponentAccessor(reader, logger, new List<ComponentSchema>());
  }
}
