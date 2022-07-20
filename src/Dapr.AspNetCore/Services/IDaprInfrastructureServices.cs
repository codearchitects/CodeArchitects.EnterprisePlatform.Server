namespace CodeArchitects.Platform.Dapr.AspNetCore.Services;

/// <summary>
/// The Dapr infrastructure service container.
/// </summary>
public interface IDaprInfrastructureServices : IDaprInfrastructureServiceProvider
{
  /// <summary>
  /// Adds a service to the service container.
  /// </summary>
  /// <typeparam name="TService">The service type.</typeparam>
  /// <param name="service">The service object.</param>
  void AddService<TService>(TService service)
    where TService : class;
}
