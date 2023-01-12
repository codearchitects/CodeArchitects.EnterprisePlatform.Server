using System.Reflection;

namespace CodeArchitects.Platform.Common.Utils;

/// <summary>
/// Extension methods using reflection.
/// </summary>
internal static class ReflectionExtensions
{
  /// <summary>
  /// Invokes a public method using reflection.
  /// </summary>
  /// <param name="instance">The object on which invoke the method.</param>
  /// <param name="methodName">The name of the method to invoke.</param>
  /// <param name="args">
  /// An argument list for the invoked method or constructor. This is an array of objects
  /// with the same number, order, and type as the parameters of the method or constructor
  /// to be invoked. If there are no parameters, parameters should be null. If the
  /// method or constructor represented by this instance takes a ref parameter (ByRef
  /// in Visual Basic), no special attribute is required for that parameter in order
  /// to invoke the method or constructor using this function. Any object in this array
  /// that is not explicitly initialized with a value will contain the default value
  /// for that object type. For reference-type elements, this value is null. For value-type
  /// elements, this value is 0, 0.0, or false, depending on the specific element type.
  /// </param>
  /// <returns>An object containing the return value of the invoked method, or null in the case of a constructor.</returns>
  public static object? InvokePublicMethod(this object instance, string methodName, params object[] args)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (methodName is null)
      throw new ArgumentNullException(nameof(methodName));

    MethodInfo? method = instance
      .GetType()
      .GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

    if (method is null)
      throw new MissingMethodException(instance.GetType().FullName, methodName);

    return method.Invoke(instance, args);
  }

  /// <summary>
  /// Invokes a non-public method using reflection.
  /// </summary>
  /// <param name="instance">The object on which invoke the method.</param>
  /// <param name="methodName">The name of the method to invoke.</param>
  /// <param name="args">
  /// An argument list for the invoked method or constructor. This is an array of objects
  /// with the same number, order, and type as the parameters of the method or constructor
  /// to be invoked. If there are no parameters, parameters should be null. If the
  /// method or constructor represented by this instance takes a ref parameter (ByRef
  /// in Visual Basic), no special attribute is required for that parameter in order
  /// to invoke the method or constructor using this function. Any object in this array
  /// that is not explicitly initialized with a value will contain the default value
  /// for that object type. For reference-type elements, this value is null. For value-type
  /// elements, this value is 0, 0.0, or false, depending on the specific element type.
  /// </param>
  /// <returns>An object containing the return value of the invoked method, or null in the case of a constructor.</returns>
  public static object? InvokeNonPublicMethod(this object instance, string methodName, params object[] args)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (methodName is null)
      throw new ArgumentNullException(nameof(methodName));

    MethodInfo? method = instance
      .GetType()
      .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

    if (method is null)
      throw new MissingMethodException(instance.GetType().FullName, methodName);

    return method.Invoke(instance, args);
  }

  /// <summary>
  /// Gets a public property using reflection.
  /// </summary>
  /// <param name="instance">The object whose property value will be returned.</param>
  /// <param name="propertyName">The name of the property to get.</param>
  /// <returns>The property value of the specified object.</returns>
  public static object? GetPublicProperty(this object instance, string propertyName)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (propertyName is null)
      throw new ArgumentNullException(nameof(propertyName));

    PropertyInfo? property = instance
      .GetType()
      .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

    if (property is null)
      throw new MissingMemberException(instance.GetType().FullName, propertyName);

    return property.GetValue(instance);
  }

  /// <summary>
  /// Sets a public property using reflection.
  /// </summary>
  /// <param name="instance">The object whose property value will be returned.</param>
  /// <param name="propertyName">The name of the property to get.</param>
  /// <param name="value">The new property value.</param>
  public static void SetPublicProperty(this object instance, string propertyName, object value)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (propertyName is null)
      throw new ArgumentNullException(nameof(propertyName));

    PropertyInfo? property = instance
      .GetType()
      .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

    if (property is null)
      throw new MissingMemberException(instance.GetType().FullName, propertyName);

    property.SetValue(instance, value);
  }

  /// <summary>
  /// Gets a non-public property using reflection.
  /// </summary>
  /// <param name="instance">The object whose property value will be returned.</param>
  /// <param name="propertyName">The name of the property to get.</param>
  /// <returns>The property value of the specified object.</returns>
  public static object? GetNonPublicProperty(this object instance, string propertyName)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (propertyName is null)
      throw new ArgumentNullException(nameof(propertyName));

    PropertyInfo? property = instance
      .GetType()
      .GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

    if (property is null)
      throw new MissingMemberException(instance.GetType().FullName, propertyName);

    return property.GetValue(instance);
  }

  /// <summary>
  /// Sets a non-public property using reflection.
  /// </summary>
  /// <param name="instance">The object whose property value will be returned.</param>
  /// <param name="propertyName">The name of the property to get.</param>
  /// <param name="value">The new property value.</param>
  public static void SetNonPublicProperty(this object instance, string propertyName, object value)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));

    PropertyInfo? property = instance
      .GetType()
      .GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

    if (property is null)
      throw new MissingMemberException(instance.GetType().FullName, propertyName);

    property.SetValue(instance, value);
  }

  /// <summary>
  /// Gets a public field using reflection.
  /// </summary>
  /// <param name="instance">The object whose field value will be returned.</param>
  /// <param name="fieldName">The name of the field to get.</param>
  /// <returns>The field value of the specified object.</returns>
  public static object? GetPublicField(this object instance, string fieldName)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (fieldName is null)
      throw new ArgumentNullException(nameof(fieldName));

    FieldInfo? field = instance
      .GetType()
      .GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

    if (field is null)
      throw new MissingFieldException(instance.GetType().FullName, fieldName);

    return field.GetValue(instance);
  }

  /// <summary>
  /// Sets a public field using reflection.
  /// </summary>
  /// <param name="instance">The object whose field value will be returned.</param>
  /// <param name="fieldName">The name of the field to get.</param>
  /// <param name="value">The new field value.</param>
  public static void SetPublicField(this object instance, string fieldName, object value)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (fieldName is null)
      throw new ArgumentNullException(nameof(fieldName));

    FieldInfo? field = instance
      .GetType()
      .GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);

    if (field is null)
      throw new MissingFieldException(instance.GetType().FullName, fieldName);

    field.SetValue(instance, value);
  }

  /// <summary>
  /// Gets a non-public field using reflection.
  /// </summary>
  /// <param name="instance">The object whose field value will be returned.</param>
  /// <param name="fieldName">The name of the field to get.</param>
  /// <returns>The field value of the specified object.</returns>
  public static object? GetNonPublicField(this object instance, string fieldName)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (fieldName is null)
      throw new ArgumentNullException(nameof(fieldName));

    FieldInfo? field = instance
      .GetType()
      .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

    if (field is null)
      throw new MissingFieldException(instance.GetType().FullName, fieldName);

    return field.GetValue(instance);
  }

  /// <summary>
  /// Sets a non-public field using reflection.
  /// </summary>
  /// <param name="instance">The object whose field value will be returned.</param>
  /// <param name="fieldName">The name of the field to get.</param>
  /// <param name="value">The new field value.</param>
  public static void SetNonPublicField(this object instance, string fieldName, object value)
  {
    if (instance is null)
      throw new ArgumentNullException(nameof(instance));
    if (fieldName is null)
      throw new ArgumentNullException(nameof(fieldName));

    FieldInfo? field = instance
      .GetType()
      .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

    if (field is null)
      throw new MissingFieldException(instance.GetType().FullName, fieldName);

    field.SetValue(instance, value);
  }
}
