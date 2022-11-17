using System.Reflection;

namespace CodeArchitects.Platform.Common.Utils;

/// <summary>
/// Contains helper methods for creating objects.
/// </summary>
internal static class CreationHelpers
{
  /// <summary>
  /// Attempts to create a class of the given type by resolving its dependencies from a service provider.
  /// </summary>
  /// <remarks></remarks>
  /// <typeparam name="T">The type of the object to create.</typeparam>
  /// <param name="services">The service provider.</param>
  /// <returns>An instance of <typeparamref name="T"/>.</returns>
  /// <exception cref="InvalidOperationException">Dependencies cannot be resolved for any of the constructors.</exception>
  /// <exception cref="ArgumentNullException">'<paramref name="services"/>' is null.</exception>
  /// <inheritdoc cref="Activator.CreateInstance(Type)"/>
  /// <inheritdoc cref="ConstructorInfo.Invoke(object[])"/>
  public static T CreateFromServices<T>(IServiceProvider services) => (T)CreateFromServices(services, typeof(T));

  /// <summary>
  /// Attempts to create a class of the given type by resolving its dependencies from a service provider.
  /// </summary>
  /// <param name="services">The service provider.</param>
  /// <param name="type">The type of the object to create.</param>
  /// <returns>An instance of <typeparamref name="T"/>.</returns>
  /// <exception cref="ArgumentNullException">Either '<paramref name="services"/>' or '<paramref name="type"/>' is null.</exception>
  /// <exception cref="MissingMethodException">'<paramref name="type"/>' has no public constructors.</exception>
  /// <exception cref="InvalidOperationException">Dependencies cannot be resolved for any of the public constructors.</exception>
  /// <inheritdoc cref="Activator.CreateInstance(Type)"/>
  /// <inheritdoc cref="ConstructorInfo.Invoke(object[])"/>
  public static object CreateFromServices(IServiceProvider services, Type type)
  {
    if (services is null)
      throw new ArgumentNullException(nameof(services));
    if (type is null)
      throw new ArgumentNullException(nameof(type));

    ConstructorInfo[] constructors = type.GetConstructors();
    if (constructors.Length == 0)
    {
      throw new MissingMethodException($"No public constructors are defined for type {type.FullName}.");
    }

    foreach (ConstructorInfo constructor in constructors.OrderByDescending(x => x.GetParameters().Length))
    {
      bool canResolve = true;
      ParameterInfo[] parameters = constructor.GetParameters();
      object[] deps = new object[parameters.Length];

      for (int i = 0; i < parameters.Length; i++)
      {
        ParameterInfo parameter = parameters[i];
        object? service = services.GetService(parameter.ParameterType);

        if (service is null)
        {
          canResolve = false;
          break;
        }

        deps[i] = service;
      }
      if (!canResolve)
      {
        continue;
      }

      return constructor.Invoke(deps);
    }

    throw new InvalidOperationException($"Cannot resolve all services to create an instance of {type}.");
  }
}
