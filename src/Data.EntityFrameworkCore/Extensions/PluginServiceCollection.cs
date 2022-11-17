using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class PluginServiceCollection : IPluginServiceCollection
{
  private readonly IServiceCollection _services;
  private bool _addModificationCommandFactory;
  private bool _addEntityQueryProvider;

  public PluginServiceCollection(IServiceCollection services)
  {
    _services = services;
    _addModificationCommandFactory = true;
    _addEntityQueryProvider = true;
  }

  public IPluginServiceCollection AddModificationInterceptor(Type implementationType)
  {
    TryAddModificationCommandFactory();
    _services.AddScoped(typeof(IModificationInterceptor), implementationType);

    return this;
  }

  public IPluginServiceCollection AddModificationInterceptor(Func<IServiceProvider, object> implementationFactory)
  {
    TryAddModificationCommandFactory();
    _services.AddScoped(typeof(IModificationInterceptor), implementationFactory);

    return this;
  }

  public IPluginServiceCollection AddQueryRootExpressionInterceptor(Type implementationType)
  {
    TryAddEntityQueryProvider();
    _services.AddScoped(typeof(IQueryRootExpressionInterceptor), implementationType);

    return this;
  }

  public IPluginServiceCollection AddQueryRootExpressionInterceptor(Func<IServiceProvider, object> implementationFactory)
  {
    TryAddEntityQueryProvider();
    _services.AddScoped(typeof(IQueryRootExpressionInterceptor), implementationFactory);

    return this;
  }

  private void TryAddModificationCommandFactory()
  {
    if (!_addModificationCommandFactory)
      return;

    _services.RemoveAll<IModificationCommandFactory>();
    _services.AddSingleton<IModificationCommandFactory, InterceptedModificationCommandFactory>();
    _addModificationCommandFactory = false;
  }

  private void TryAddEntityQueryProvider()
  {
    if (!_addEntityQueryProvider)
      return;

    _services.RemoveAll<IAsyncQueryProvider>();
    _services.AddScoped<IAsyncQueryProvider, InterceptedEntityQueryProvider>();
    _services.AddScoped<IExpressionRewriter, ExpressionRewriter>();
    _addEntityQueryProvider = false;
  }
}
