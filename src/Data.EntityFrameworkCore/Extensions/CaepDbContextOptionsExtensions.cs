using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Extension methods for <see cref="DbContextOptionsBuilder"/> to register the CAEP extension.
/// </summary>
public static class CaepDbContextOptionsExtensions
{
  /// <summary>
  /// Use the CAEP extension.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <returns>The same <see cref="DbContextOptionsBuilder"/> for further configuration.</returns>
  public static DbContextOptionsBuilder UseCaep(this DbContextOptionsBuilder builder)
  {
    ArgumentNullException.ThrowIfNull(builder);

    (builder as IDbContextOptionsBuilderInfrastructure).AddOrUpdateExtension(new CaepExtension(Enumerable.Empty<ICaepExtensionPlugin>()));

    return builder;
  }

  /// <summary>
  /// Use the CAEP extension.
  /// </summary>
  /// <param name="builder">The options builder.</param>
  /// <param name="buildAction">Action used to configure the CAEP options builder.</param>
  /// <returns>The same <see cref="DbContextOptionsBuilder"/> for further configuration.</returns>
  public static DbContextOptionsBuilder UseCaep(this DbContextOptionsBuilder builder, Action<CaepOptionsBuilder> buildAction)
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(buildAction);

    CaepOptionsBuilder dataBuilder = new();
    buildAction(dataBuilder);

    (builder as IDbContextOptionsBuilderInfrastructure).AddOrUpdateExtension(new CaepExtension(dataBuilder.Plugins));

    return builder;
  }
}
