using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal class PluginServiceCollection : IPluginServiceCollection
{
  private static readonly MethodInfo s_getRequiredServiceMethod = typeof(ServiceProviderServiceExtensions).GetRequiredMethod(
    name: nameof(ServiceProviderServiceExtensions.GetRequiredService),
    bindingAttr: BindingFlags.Static | BindingFlags.Public,
    types: new[] { typeof(IServiceProvider) });

  private static readonly PropertyInfo s_applicationServiceProviderProperty = typeof(AggregateServiceProvider).GetRequiredProperty(
    name: nameof(AggregateServiceProvider.ApplicationServiceProvider),
    bindingAttr: BindingFlags.Instance | BindingFlags.Public);

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

  public ServiceDescriptor this[int index]
  {
    get => ((IList<ServiceDescriptor>)_services)[index];
    set => ((IList<ServiceDescriptor>)_services)[index] = value;
  }

  public int Count => _services.Count;

  public bool IsReadOnly => _services.IsReadOnly;

  public void Add(ServiceDescriptor item)
  {
    _services.Add(GetDescriptor(item));

    static ServiceDescriptor GetDescriptor(ServiceDescriptor item)
    {
      if (item.ImplementationType is { } implementationType)
      {
        ConstructorInfo? constructor = implementationType
          .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
          .OrderByDescending(constructor => constructor.GetParameters().Length)
          .FirstOrDefault();

        if (constructor is null)
          return item;

        ParameterInfo[] parameters = constructor.GetParameters();
        if (!parameters.Any(parameter => parameter.IsDefined(typeof(ApplicationServiceAttribute))))
          return item;

        // Something like this
        // serviceProvider =>
        // {
        //   IServiceProvider applicationServiceProvider = serviceProvider
        //     .GetRequiredService<AggregateServiceProvider>()
        //     .ApplicationServiceProvider;
        //   
        //   return new(
        //     ...
        //     serviceProvider.GetRequiredService<...>(),
        //     ...
        //     applicationServiceProvider.GetRequiredService<...>(),
        //     ...);
        // }

        ParameterExpression serviceProvider = Expression.Parameter(typeof(IServiceProvider), nameof(serviceProvider));

        ParameterExpression applicationServiceProvider = Expression.Variable(typeof(IServiceProvider), nameof(applicationServiceProvider));

        Expression[] arguments = parameters.Select(parameter => Expression.Call(
          instance: null,
          method: s_getRequiredServiceMethod.MakeGenericMethod(parameter.ParameterType),
          arguments: parameter.IsDefined(typeof(ApplicationServiceAttribute))
            ? applicationServiceProvider
            : serviceProvider))
          .ToArray();

        Expression<Func<IServiceProvider, object>> factoryExpression = Expression.Lambda<Func<IServiceProvider, object>>(
          body: Expression.Block(
            type: typeof(object),
            variables: new[] { applicationServiceProvider },
            expressions: new Expression[]
            {
              Expression.Assign(
                left: applicationServiceProvider,
                right: Expression.Property(
                  expression: Expression.Call(
                    instance: null,
                    method: s_getRequiredServiceMethod.MakeGenericMethod(typeof(AggregateServiceProvider)),
                    arguments: serviceProvider),
                  property: s_applicationServiceProviderProperty)),
              Expression.New(
                constructor: constructor,
                arguments: arguments)
            }),
          parameters: serviceProvider);

        return new ServiceDescriptor(item.ServiceType, factoryExpression.Compile(), item.Lifetime);
      }

      if (item.ImplementationFactory is { } implementationFactory)
      {
        return new ServiceDescriptor(item.ServiceType, sp => implementationFactory(sp.GetRequiredService<AggregateServiceProvider>()), item.Lifetime);
      }

      return item;
    }
  }

  public void Clear()
  {
    _services.Clear();
  }

  public bool Contains(ServiceDescriptor item)
  {
    return _services.Contains(item);
  }

  public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
  {
    _services.CopyTo(array, arrayIndex);
  }

  public IEnumerator<ServiceDescriptor> GetEnumerator()
  {
    return _services.GetEnumerator();
  }

  public int IndexOf(ServiceDescriptor item)
  {
    return _services.IndexOf(item);
  }

  public void Insert(int index, ServiceDescriptor item)
  {
    _services.Insert(index, item);
  }

  public bool Remove(ServiceDescriptor item)
  {
    return _services.Remove(item);
  }

  public void RemoveAt(int index)
  {
    _services.RemoveAt(index);
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_services).GetEnumerator();
  }
}
