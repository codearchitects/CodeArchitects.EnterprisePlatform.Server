using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.Features.SoftDelete;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.SoftDelete;

internal class SoftDeleteDataPlugin : IDataExtensionPlugin
{
  private readonly ISoftDeleteDescriptor _descriptor;

  public SoftDeleteDataPlugin(ISoftDeleteDescriptor descriptor)
  {
    _descriptor = descriptor;
  }

  public void ApplyServices(IPluginServiceCollection services)
  {
    if (_descriptor.SoftDeleteContextType is not null)
    {
      services.AddScoped(typeof(ISoftDeleteContext), _descriptor.SoftDeleteContextType);
    }
    else if (_descriptor.SoftDeleteContextImplementationFactory is not null)
    {
      services.AddScoped(typeof(ISoftDeleteContext), _descriptor.SoftDeleteContextImplementationFactory);
    }

    services.AddScoped<IInterceptor, SaveChangesInterceptor>();

    services.AddScoped<IConventionSetPlugin, ConventionSetPlugin>();

    services
      .AddModificationInterceptor<ModificationInterceptor>()
      .AddQueryRootExpressionInterceptor<QueryRootExpressionInterceptor>();
  }
}
