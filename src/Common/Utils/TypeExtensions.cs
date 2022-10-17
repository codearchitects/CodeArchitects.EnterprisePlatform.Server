namespace CodeArchitects.Platform.Common.Utils;

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
  public static bool ImplementsGenericInterface(this Type type, Type genericInterfaceType)
  {
    if (type is null)
      throw new ArgumentNullException(nameof(type));
    if (genericInterfaceType is null)
      throw new ArgumentNullException(nameof(genericInterfaceType));
    if (!genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType)
      throw new ArgumentException("The interface type must be a generic interface.", nameof(genericInterfaceType));

    foreach (Type interfaceType in type.GetInterfaces())
    {
      if (!interfaceType.IsGenericType)
        continue;

      if (interfaceType.GetGenericTypeDefinition() == genericInterfaceType)
        return true;
    }

    return false;
  }

  /// <summary>
  /// Checks if a type implements a generic interface exactly once.
  /// </summary>
  /// <param name="type">The type to check.</param>
  /// <param name="genericInterfaceType">The generic interface type.</param>
  /// <returns>True if the type implements the interface exactly once, false otherwise.</returns>
  public static bool ImplementsGenericInterfaceExactlyOnce(this Type type, Type genericInterfaceType)
  {
    if (type is null)
      throw new ArgumentNullException(nameof(type));
    if (genericInterfaceType is null)
      throw new ArgumentNullException(nameof(genericInterfaceType));
    if (!genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType)
      throw new ArgumentException("The interface type must be a generic interface.", nameof(genericInterfaceType));

    bool implementsInterface = false;
    foreach (Type interfaceType in type.GetInterfaces())
    {
      if (!interfaceType.IsGenericType)
        continue;

      if (interfaceType.GetGenericTypeDefinition() == genericInterfaceType)
      {
        if (implementsInterface)
          return false;
        implementsInterface = true;
      }
    }

    return implementsInterface;
  }

  /// <summary>
  /// Given a type, returns all the interfaces it implements which generic type matches the provided interface types.
  /// </summary>
  /// <param name="type">The type.</param>
  /// <param name="genericInterfaceTypes">The generic interface type.</param>
  /// <returns>An enumerable of interface types.</returns>
  public static IEnumerable<Type> GetGenericInterfaces(this Type type, params Type[] genericInterfaceTypes)
  {
    if (type is null)
      throw new ArgumentNullException(nameof(type));
    if (genericInterfaceTypes is null)
      throw new ArgumentNullException(nameof(genericInterfaceTypes));
    foreach (Type genericInterfaceType in genericInterfaceTypes)
    {
      if (!genericInterfaceType.IsInterface || !genericInterfaceType.IsGenericType)
        throw new ArgumentException($"'{nameof(genericInterfaceTypes)}' is supposed to contain generic interface types only.", nameof(genericInterfaceTypes));
    }

    return type
      .GetInterfaces()
      .Where(x => x.IsGenericType && genericInterfaceTypes.Contains(x.GetGenericTypeDefinition()));
  }
}
