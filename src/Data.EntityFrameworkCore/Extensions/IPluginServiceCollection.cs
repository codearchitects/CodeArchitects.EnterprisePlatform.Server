namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

public interface IPluginServiceCollection
{
  IPluginServiceCollection AddModificationInterceptor(Type implementationType);
  IPluginServiceCollection AddModificationInterceptor(Func<IServiceProvider, object> implementationFactory);

  IPluginServiceCollection AddQueryRootExpressionInterceptor(Type implementationType);
  IPluginServiceCollection AddQueryRootExpressionInterceptor(Func<IServiceProvider, object> implementationFactory);
}

public static class PluginServiceCollectionExtensions
{
  public static IPluginServiceCollection AddModificationInterceptor<TImplementation>(this IPluginServiceCollection services)
    where TImplementation : IModificationInterceptor
  {
    return services.AddModificationInterceptor(typeof(TImplementation));
  }

  public static IPluginServiceCollection AddQueryRootExpressionInterceptor<TImplementation>(this IPluginServiceCollection services)
    where TImplementation : IQueryRootExpressionInterceptor
  {
    return services.AddQueryRootExpressionInterceptor(typeof(TImplementation));
  }
}