using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeArchitects.Platform.Common.Utils
{
  /// <summary>
  /// Extension methods for the <see cref="Type"/> class.
  /// </summary>
  internal static class TypeExtensions
  {
    /// <summary>
    /// Checks if a type implements a generic interface at least once.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="genericInterfaceType">The generic interface type.</param>
    /// <returns>True if the type implements the interface, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Either '<paramref name="type"/>' or '<paramref name="genericInterfaceType"/>' is null.</exception>
    /// <exception cref="ArgumentException">'<paramref name="genericInterfaceType"/>' is not a generic interface type.</exception>
    /// <inheritdoc cref="Type.GetInterfaces"/>
    public static bool ImplementsGenericInterface(this Type type, Type genericInterfaceType)
    {
      if (type is null) throw new ArgumentNullException(nameof(type));
      if (genericInterfaceType is null) throw new ArgumentNullException(nameof(genericInterfaceType));
      if (!genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType) throw new ArgumentException("The interface type must be a generic interface.", nameof(genericInterfaceType));

      return type
        .GetInterfaces()
        .Where(x => x.IsGenericType)
        .Select(x => x.GetGenericTypeDefinition())
        .Contains(genericInterfaceType);
    }

    /// <summary>
    /// Checks if a type implements a generic interface exactly once.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="genericInterfaceType">The generic interface type.</param>
    /// <returns>True if the type implements the interface exactly once, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Either '<paramref name="type"/>' or '<paramref name="genericInterfaceType"/>' is null.</exception>
    /// <exception cref="ArgumentException">'<paramref name="genericInterfaceType"/>' is not a generic interface type.</exception>
    /// <inheritdoc cref="Type.GetInterfaces"/>
    public static bool ImplementsGenericInterfaceExactlyOnce(this Type type, Type genericInterfaceType)
    {
      if (type is null) throw new ArgumentNullException(nameof(type));
      if (genericInterfaceType is null) throw new ArgumentNullException(nameof(genericInterfaceType));
      if (!genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType) throw new ArgumentException("The interface type must be a generic interface.", nameof(genericInterfaceType));

      return type
        .GetInterfaces()
        .Where(x => x.IsGenericType)
        .Select(x => x.GetGenericTypeDefinition())
        .Count(x => x == genericInterfaceType) == 1;
    }

    /// <summary>
    /// Given a type, returns all the interfaces it implements which generic type matches the provided interface type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="genericInterfaceType">The generic interface type.</param>
    /// <returns>An enumerable of interface types.</returns>
    /// <exception cref="ArgumentNullException">Either '<paramref name="type"/>' or '<paramref name="genericInterfaceType"/>' is null.</exception>
    /// <exception cref="ArgumentException">'<paramref name="genericInterfaceType"/>' is not a generic interface type.</exception>
    /// <inheritdoc cref="Type.GetInterfaces"/>
    public static IEnumerable<Type> GetGenericInterfaces(this Type type, Type genericInterfaceType)
    {
      if (type is null) throw new ArgumentNullException(nameof(type));
      if (genericInterfaceType is null) throw new ArgumentNullException(nameof(genericInterfaceType));
      if (!genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType) throw new ArgumentException("The interface type must be a generic interface.", nameof(genericInterfaceType));

      return type
        .GetInterfaces()
        .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericInterfaceType);
    }
  }
}
