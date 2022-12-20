using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.DependencyInjection;
using CodeArchitects.Platform.Data.AdoNet.Interceptors;
using System.Data.Common;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for <see cref="IAdoNetConfigurationBuilderWithProvider"/>.
/// </summary>
public static class AdoNetConfigurationBuilderExtensions
{
  /// <summary>
  /// Scans the specifies assembly and looks for a <see cref="ModelConfiguration"/> type to use and registers any found <see cref="ICommandInterceptor{TCommand}"/> implementation.
  /// </summary>
  /// <typeparam name="TMarker">A type used to mark the assembly to scan.</typeparam>
  /// <param name="builder">The ADO.NET configuration builder.</param>
  /// <param name="serviceTypes">Specifies which services to look for.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> that can be used for further configuration.</returns>
  public static IAdoNetConfigurationBuilderWithProvider ScanServicesFromAssemblyContaining<TMarker>(this IAdoNetConfigurationBuilderWithProvider builder, AdoNetServiceTypes serviceTypes = AdoNetServiceTypes.All)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.ScanServicesFromAssembly(typeof(TMarker).Assembly, serviceTypes);
  }

  /// <summary>
  /// Specifies the model configuration to use to build the persistence model.
  /// </summary>
  /// <typeparam name="TModelConfiguration">The model configuration type.</typeparam>
  /// <param name="builder">The ADO.NET configuration builder.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> that can be used for further configuration.</returns>
  public static IAdoNetConfigurationBuilderWithProvider UseModel<TModelConfiguration>(this IAdoNetConfigurationBuilderWithProvider builder)
    where TModelConfiguration : ModelConfiguration
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.UseModel(typeof(TModelConfiguration));
  }

  /// <summary>
  /// Adds a command interceptor to the services.
  /// </summary>
  /// <typeparam name="TInterceptor">The interceptor type.</typeparam>
  /// <param name="builder">The ADO.NET configuration builder.</param>
  /// <returns>An <see cref="IAdoNetConfigurationBuilderWithProvider"/> that can be used for further configuration.</returns>
  public static IAdoNetConfigurationBuilderWithProvider AddCommandInterceptor<TInterceptor>(this IAdoNetConfigurationBuilderWithProvider builder)
    where TInterceptor : ICommandInterceptor<DbCommand>
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.AddCommandInterceptor(typeof(TInterceptor));
  }
}
