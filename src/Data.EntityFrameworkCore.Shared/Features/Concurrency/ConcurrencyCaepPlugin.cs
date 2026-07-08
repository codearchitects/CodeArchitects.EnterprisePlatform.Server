using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Concurrency;

internal class ConcurrencyCaepPlugin : ICaepExtensionPlugin
{
  private readonly ServiceDescriptor _tokenProviderDescriptor;

  public ConcurrencyCaepPlugin(ServiceDescriptor tokenProviderDescriptor)
  {
    _tokenProviderDescriptor = tokenProviderDescriptor;
  }

  public void ApplyServices(IPluginServiceCollection services)
  {
    services.Add(_tokenProviderDescriptor);

    ConventionSetPlugin plugin = new();
    services.AddScoped(typeof(IConventionSetPlugin), sp => plugin);
  }
}
