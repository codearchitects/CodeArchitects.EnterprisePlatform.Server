namespace CodeArchitects.Platform.Dapr.AspNetCore.Services;

/// <summary>
/// The Dapr infrastructure service provider.
/// </summary>
public interface IDaprInfrastructureServiceProvider
{
  /// <summary>
  /// Retrieves a previously registered service.
  /// </summary>
  /// <typeparam name="TService">The service type.</typeparam>
  /// <returns>The service object.</returns>
  TService GetService<TService>();
}
