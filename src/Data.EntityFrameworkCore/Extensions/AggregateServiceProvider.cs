using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class AggregateServiceProvider : IServiceProvider
{
  private readonly IServiceProvider _internalServiceProvider;
  private IServiceProvider? _applicationServiceProvider;

  public AggregateServiceProvider(IServiceProvider internalServiceProvider)
  {
    _internalServiceProvider = internalServiceProvider;
  }

  public IServiceProvider ApplicationServiceProvider => _applicationServiceProvider ??= _internalServiceProvider
    .GetRequiredService<IDbContextOptions>()
    .FindExtension<CoreOptionsExtension>()?
    .ApplicationServiceProvider ?? throw new InvalidOperationException("Could not resolve the application service provider.");

  public object? GetService(Type serviceType)
  {
    if (_internalServiceProvider.GetService(serviceType) is { } service)
      return service;

    return ApplicationServiceProvider.GetService(serviceType);
  }
}
