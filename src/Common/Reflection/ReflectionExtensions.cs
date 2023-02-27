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

  internal static object? GetValue(this MemberInfo member, object? instance, object?[]? arguments = null)
  {
    switch (member)
    {
      case PropertyInfo property:
        return property.GetValue(instance, arguments);
      case FieldInfo field:
        if (arguments is not null)
          throw new ArgumentException("The index value array should be null for fields.");
        return field.GetValue(instance);
      default:
        throw new ArgumentException($"Expected a PropertyInfo or a FieldInfo, but got '{member.GetType().Name}'.");
    }
  }

  internal static void SetValue(this MemberInfo member, object? instance, object? value, object?[]? arguments = null)
  {
    switch (member)
    {
      case PropertyInfo property:
        property.SetValue(instance, value, arguments);
        return;
      case FieldInfo field:
        if (arguments is not null)
          throw new ArgumentException("The index value array should be null for fields.");
        field.SetValue(instance, value);
        return;
      default:
        throw new ArgumentException($"Expected a PropertyInfo or a FieldInfo, but got '{member.GetType().Name}'.");
    }
  }

  public static bool TryGetBackingFieldByConvention(this PropertyInfo property, BackingFieldNameConvention conventions, [NotNullWhen(true)] out FieldInfo? backingField)
  {
    Type type = property.DeclaringType!;
    string propertyName = property.Name;

    backingField = null;
    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
    {
      string fieldName = field.Name;
      bool match =
        (conventions & BackingFieldNameConvention.AutoGen) != 0          && propertyName.MatchesAutoGenConvention(fieldName)          ||
        (conventions & BackingFieldNameConvention.CamelCase) != 0        && propertyName.MatchesCamelCaseConvention(fieldName)        ||
        (conventions & BackingFieldNameConvention.UnderscorePrefix) != 0 && propertyName.MatchesUnderscorePrefixConvention(fieldName) ||
        (conventions & BackingFieldNameConvention.MemberPrefix) != 0     && propertyName.MatchesMemberPrefixConvention(fieldName);

      if (match)
      {
        if (backingField is not null)
          throw new AmbiguousMatchException($"Found multiple backing fields for property '{property.Name}' using convention '{conventions}'.");

        backingField = field;
      }
    }

    return backingField is not null;
  }
}
