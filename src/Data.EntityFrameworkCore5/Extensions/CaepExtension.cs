using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class CaepExtension : CaepExtensionBase
{
  public CaepExtension(IEnumerable<ICaepExtensionPlugin> plugins)
    : base(plugins)
  {
  }

  public override DbContextOptionsExtensionInfo Info => new DataExtensionInfo(this);

  private sealed class DataExtensionInfo : DbContextOptionsExtensionInfo
  {
    public DataExtensionInfo(CaepExtension extension)
      : base(extension)
    {
    }

    public override bool IsDatabaseProvider => false;

    public override string LogFragment => "CodeArchitects:Caep";

    public override long GetServiceProviderHashCode()
    {
      return 0;
    }

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
    }
  }
}
