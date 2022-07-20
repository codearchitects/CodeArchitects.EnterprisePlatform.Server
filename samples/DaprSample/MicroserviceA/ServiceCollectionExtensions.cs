namespace MicroserviceA;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddDaprInfrastructure(this IServiceCollection services, IConfiguration configuration)
  {
    services
      .AddDaprInfrastructure(options => options
        .SetConfiguration(configuration))
      .AddMessaging(messaging => messaging
        .ScanAssembly(typeof(Program).Assembly))
      .AddStateStore();
    return services;
  }

  public static IServiceCollection AddSwagger(this IServiceCollection services)
  {
    return services
      .AddEndpointsApiExplorer()
      .AddSwaggerGen();
  }
}
