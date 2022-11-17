using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class DataExtension : DataExtensionBase
{
  public override DbContextOptionsExtensionInfo Info => new DataExtensionInfo(this);

  private class DataExtensionInfo : DbContextOptionsExtensionInfo
  {
    public DataExtensionInfo(DataExtension extension)
      : base(extension)
    {
    }

    public override bool IsDatabaseProvider => false;

    public override string LogFragment => "CodeArchitects:Data";

    public override long GetServiceProviderHashCode()
    {
      return 0;
    }

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
    }
  }
}
