using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class CaepExtension : CaepExtensionBase
{
  public CaepExtension(IEnumerable<ICaepExtensionPlugin> plugins)
    : base(plugins)
  {
  }

  public override DbContextOptionsExtensionInfo Info => new CaepExtensionInfo(this);

  private class CaepExtensionInfo : DbContextOptionsExtensionInfo
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
