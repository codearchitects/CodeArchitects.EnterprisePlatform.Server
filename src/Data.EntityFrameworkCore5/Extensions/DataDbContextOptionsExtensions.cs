using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public static class DataDbContextOptionsExtensions
{
  public static DbContextOptionsBuilder UseData(this DbContextOptionsBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    (builder as IDbContextOptionsBuilderInfrastructure).AddOrUpdateExtension(new DataExtension());

    return builder;
  }
}
