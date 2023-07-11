using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;
using CodeArchitects.Platform.Data.Features.Multitenancy;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class MultitenancyCaepPlugin : ICaepExtensionPlugin
{
  private readonly IMultitenancyDescriptor _descriptor;

  public MultitenancyCaepPlugin(IMultitenancyDescriptor descriptor)
  {
    _descriptor = descriptor;
  }

  public void ApplyServices(IPluginServiceCollection services)
  {
    if (_descriptor.MultitenancyContextType is not null)
    {
      services.AddScoped(_descriptor.MultitenancyContextType);
      services.AddScoped(sp => new MultitenancyContextWrapper((IMultitenancyContext)sp.GetRequiredService(_descriptor.MultitenancyContextType)));
    }
    else if (_descriptor.MultitenancyContextImplementationFactory is not null)
    {
      services.AddScoped(sp => new MultitenancyContextWrapper(_descriptor.MultitenancyContextImplementationFactory(sp)));
    }

    services.AddScoped<IMultitenancyContext>(sp => sp.GetRequiredService<MultitenancyContextWrapper>());
    services.AddScoped<IMultitenancyContextBypasser>(sp => sp.GetRequiredService<MultitenancyContextWrapper>());

    services.AddScoped<IInterceptor, SaveChangesInterceptor>();

    ConventionSetPlugin plugin = new(_descriptor.TenantIdType);
    services.AddScoped(typeof(IConventionSetPlugin), sp => plugin);

    services
      .AddModificationInterceptor<ModificationInterceptor>()
      .AddQueryRootExpressionInterceptor<QueryRootExpressionInterceptor>();
  }
}
