using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public static class DataDbContextOptionsExtensions
{
  public static DbContextOptionsBuilder UseData(this DbContextOptionsBuilder builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    (builder as IDbContextOptionsBuilderInfrastructure).AddOrUpdateExtension(new DataExtension(Enumerable.Empty<IDataExtensionPlugin>()));

    return builder;
  }

  public static DbContextOptionsBuilder UseData(this DbContextOptionsBuilder builder, Action<DataOptionsBuilder> buildAction)
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(buildAction);

    DataOptionsBuilder dataBuilder = new();
    buildAction(dataBuilder);

    (builder as IDbContextOptionsBuilderInfrastructure).AddOrUpdateExtension(new DataExtension(dataBuilder.Plugins));

    return builder;
  }
}
