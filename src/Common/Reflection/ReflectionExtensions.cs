using CodeArchitects.Platform.Common.Reflection;
using CodeArchitects.Platform.Common.Utils;
using System.Diagnostics.CodeAnalysis;

namespace System.Reflection;

internal static class ReflectionExtensions
{
  public static FieldInfo GetRequiredField(this Type type, string name)
  {
    FieldInfo? field = type.GetField(name);
    return field ?? throw new MissingFieldException(type.Name, name);
  }

  public static FieldInfo GetRequiredField(this Type type, string name, BindingFlags bindingAttr)
  {
    FieldInfo? field = type.GetField(name, bindingAttr);
    return field ?? throw new MissingFieldException(type.Name, name);
  }

  public static PropertyInfo GetRequiredProperty(this Type type, string name)
  {
    PropertyInfo? property = type.GetProperty(name);

    return property ?? throw new MissingMemberException(type.Name, name);
  }

  public static PropertyInfo GetRequiredProperty(this Type type, string name, BindingFlags bindingAttr)
  {
    PropertyInfo? property = type.GetProperty(
      name: name,
      bindingAttr: bindingAttr,
      binder: null,
      returnType: null,
      types: Array.Empty<Type>(),
      modifiers: null);

    return property ?? throw new MissingMemberException(type.Name, name);
  }

  public static PropertyInfo GetRequiredProperty(this Type type, string name, BindingFlags bindingAttr, Type[] types)
  {
    PropertyInfo? property = type.GetProperty(
      name: name,
      bindingAttr: bindingAttr,
      binder: null,
      returnType: null,
      types: types,
      modifiers: null);

    return property ?? throw new MissingPropertyException(type.Name, name);
  }

  public static MethodInfo GetRequiredMethod(this Type type, string name)
  {
    MethodInfo? method = type.GetMethod(name);

    return method ?? throw new MissingMethodException(type.Name, name);
  }

  public static MethodInfo GetRequiredMethod(this Type type, string name, BindingFlags bindingAttr)
  {
    MethodInfo? method = type.GetMethod(
      name: name,
      bindingAttr: bindingAttr);

    return method ?? throw new MissingMethodException(type.Name, name);
  }

  public static MethodInfo GetRequiredMethod(this Type type, string name, BindingFlags bindingAttr, Type[] types)
  {
    MethodInfo? method = type.GetMethod(
      name: name,
      bindingAttr: bindingAttr,
      binder: null,
      types: types,
      modifiers: null);

    return method ?? throw new MissingMethodException(type.Name, name);
  }

  public static ConstructorInfo GetRequiredConstructor(this Type type)
  {
    ConstructorInfo[] constructors = type.GetConstructors();

    return constructors.Length == 1
      ? constructors[0]
      : throw new AmbiguousMatchException();
  }

  public static ConstructorInfo GetRequiredConstructor(this Type type, BindingFlags bindingAttr)
  {
    ConstructorInfo[] constructors = type.GetConstructors(bindingAttr);

    return constructors.Length == 1
      ? constructors[0]
      : throw new AmbiguousMatchException();
  }

  public static ConstructorInfo GetRequiredConstructor(this Type type, BindingFlags bindingAttr, Type[] types)
  {
    ConstructorInfo? constructor = type.GetConstructor(
      bindingAttr: bindingAttr,
      binder: null,
      types: types,
      modifiers: null);

    return constructor ?? throw new MissingMethodException(type.Name, GetConstructorName(bindingAttr));

    static string GetConstructorName(BindingFlags bindingAttr)
    {
      return (bindingAttr & BindingFlags.Static) != 0
        ? ConstructorInfo.TypeConstructorName
        : ConstructorInfo.ConstructorName;
    }
  }

  public static bool TryGetBackingFieldByConvention(this PropertyInfo property, [NotNullWhen(true)] out FieldInfo? backingField)
  {
    Type type = property.DeclaringType!;
    string propertyName = property.Name;

    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
    {
      string fieldName = field.Name;
      bool match =
        propertyName.MatchesBackingFieldConvention(fieldName)     ||
        propertyName.MatchesCamelCaseConvention(fieldName)        ||
        propertyName.MatchesUnderscorePrefixConvention(fieldName) ||
        propertyName.MatchesMemberPrefixConvention(fieldName);

      if (match)
      {
        backingField = field;
        return true;
      }
    }

    backingField = null;
    return false;
  }
}
