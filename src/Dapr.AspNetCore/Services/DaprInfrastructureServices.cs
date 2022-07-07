namespace CodeArchitects.Platform.Dapr.AspNetCore.Services;

internal class DaprInfrastructureServices : IDaprInfrastructureServices
{
  private readonly Dictionary<Type, object> _services;

  public DaprInfrastructureServices(Dictionary<Type, object> services)
  {
    _services = services;
  }

  public TService GetService<TService>()
  {
    if (!_services.TryGetValue(typeof(TService), out object? service))
      throw new InvalidOperationException($"Service '{typeof(TService).Name}' was not registered.");

    return (TService)service;
  }

  public void AddService<TService>(TService service)
    where TService : class
  {
    _services.Add(typeof(TService), service);
  }

  public static DaprInfrastructureServices Create()
  {
    return new DaprInfrastructureServices(new Dictionary<Type, object>());
  }
}
