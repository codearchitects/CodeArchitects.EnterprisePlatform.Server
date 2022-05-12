using CodeArchitects.Platform.Dapr.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

internal class DaprConfiguration : IDaprConfiguration, IDaprConfigurationBuilder
{
  private readonly IComponentReader _componentReader;
  private readonly IConfigurationSection _configuration;
  private readonly ILoggerAccessor _loggerAccessor;
  private readonly Dictionary<Type, object> _sections;
  private readonly List<ComponentSchema> _components;

  public DaprConfiguration(IComponentReader componentReader, IConfigurationSection configuration, ILoggerAccessor loggerAccessor, Dictionary<Type, object> sections, List<ComponentSchema> components)
  {
    _componentReader = componentReader;
    _configuration = configuration;
    _loggerAccessor = loggerAccessor;
    _sections = sections;
    _components = components;
  }

  public IReadOnlyList<ComponentSchema> Components => _components;

  public IDaprConfiguration Configuration => this;

  public void AddComponents(IFileProvider componentsFolder)
  {
    ILogger logger = _loggerAccessor.Logger;

    IDirectoryContents contents = componentsFolder.GetDirectoryContents("/");
    if (!contents.Exists)
      return;

    foreach (IFileInfo file in contents)
    {
      if (!file.Exists || file.IsDirectory)
        continue;

      try
      {
        ComponentSchema component = _componentReader.FromFile(file);
        _components.Add(component);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "Could not read component in file {0}", file.PhysicalPath ?? file.Name);
      }
    }
  }

  public TSection AddSection<TSection>(string key)
    where TSection : class, new()
  {
    TSection section = new TSection();
    _configuration.Bind(key, section);

    _sections[typeof(TSection)] = section;

    return section;
  }

  public TSection? GetSection<TSection>()
    where TSection : class
  {
    return _sections.TryGetValue(typeof(TSection), out object? section)
      ? (TSection)section
      : null;
  }

  public static DaprConfiguration Create(IComponentReader componentReader, IConfigurationSection configurationSection, ILoggerAccessor loggerAccessor)
  {
    Dictionary<Type, object> sections = new();
    List<ComponentSchema> components = new();

    return new DaprConfiguration(componentReader, configurationSection, loggerAccessor, sections, components);
  }
}
