using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class CaepExtension : CaepExtensionBase
{
  public CaepExtension(IEnumerable<ICaepExtensionPlugin> plugins)
    : base(plugins)
  {
  }

  public override DbContextOptionsExtensionInfo Info => new CaepExtensionInfo(this);

  public override void ApplyServices(IServiceCollection services)
  {
    base.ApplyServices(services);

    services.TryAddSingleton<IMultitenancyContextBypasser, NullMultitenancyContextBypasser>();
  }

  private sealed class CaepExtensionInfo : DbContextOptionsExtensionInfo
  {
    public CaepExtensionInfo(CaepExtension extension)
      : base(extension)
    {
    }

    public override bool IsDatabaseProvider => false;

    public override string LogFragment => "CodeArchitects:Caep";

    public override int GetServiceProviderHashCode()
    {
      return 0;
    }

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
    }

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
    {
      return true;
    }
  }
}
