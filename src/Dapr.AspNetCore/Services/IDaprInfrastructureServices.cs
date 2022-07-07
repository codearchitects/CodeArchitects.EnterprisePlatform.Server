namespace CodeArchitects.Platform.Dapr.AspNetCore.Services;

public interface IDaprInfrastructureServices : IDaprInfrastructureServiceProvider
{
  void AddService<TService>(TService service)
    where TService : class;
}
