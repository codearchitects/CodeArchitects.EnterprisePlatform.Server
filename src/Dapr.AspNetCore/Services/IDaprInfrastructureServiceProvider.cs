namespace CodeArchitects.Platform.Dapr.AspNetCore.Services;

public interface IDaprInfrastructureServiceProvider
{
  TService GetService<TService>();
}
