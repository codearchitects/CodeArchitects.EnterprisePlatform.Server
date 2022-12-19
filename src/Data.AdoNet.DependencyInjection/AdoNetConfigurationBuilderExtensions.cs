using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.DependencyInjection;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using System.Data.Common;

namespace Microsoft.Extensions.DependencyInjection;

public static class AdoNetConfigurationBuilderExtensions
{
  public static IAdoNetConfigurationBuilderWithProvider ScanServicesFromAssemblyContaining<TMarker>(this IAdoNetConfigurationBuilderWithProvider builder, AdoNetServiceTypes serviceTypes = AdoNetServiceTypes.All)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.ScanServicesFromAssembly(typeof(TMarker).Assembly, serviceTypes);
  }

  public static IAdoNetConfigurationBuilderWithProvider UseModel<TModelConfiguration>(this IAdoNetConfigurationBuilderWithProvider builder)
    where TModelConfiguration : ModelConfiguration
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.UseModel(typeof(TModelConfiguration));
  }

  public static IAdoNetConfigurationBuilderWithProvider AddCommandInterceptor<TInterceptor>(this IAdoNetConfigurationBuilderWithProvider builder)
    where TInterceptor : ICommandInterceptor<DbCommand>
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.AddCommandInterceptor(typeof(TInterceptor));
  }
}
