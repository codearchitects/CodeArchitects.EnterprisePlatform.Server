using Mapster;
using MapsterMapper;

namespace CodeArchitects.Platform.Data.Mapster;

/// <summary>
/// A <see cref="ServiceMapper"/> on the data layer configuration: it exposes the service provider to the
/// mapping, which <c>PreserveTracking</c> relies on.
/// </summary>
internal sealed class DataMapper(IServiceProvider serviceProvider, TypeAdapterConfig config)
  : ServiceMapper(serviceProvider, config), IDataMapper
{
}

/// <summary>
/// Holds the data layer configuration without registering <see cref="TypeAdapterConfig"/> itself, which
/// belongs to the application.
/// </summary>
internal sealed class DataMapsterConfiguration(TypeAdapterConfig config)
{
  public TypeAdapterConfig Config { get; } = config;
}
