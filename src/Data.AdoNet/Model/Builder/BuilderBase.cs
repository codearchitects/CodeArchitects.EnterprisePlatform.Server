using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Reflection;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class BuilderBase
{
  protected static bool TryGetMemberAndTypeFromExpression(Expression body, [NotNullWhen(true)] out MemberInfo? member, [NotNullWhen(true)] out Type? type)
  {
    if (body is not MemberExpression memberExpression)
    {
      member = null;
      type = null;
      return false;
    }

    if (memberExpression.Expression is ParameterExpression)
    {
      if (memberExpression.Member is PropertyInfo property)
      {
        member = property;
        type = property.PropertyType;
        return true;
      }
      else if (memberExpression.Member is FieldInfo field)
      {
        member = field;
        type = field.FieldType;
        return true;
      }
    }

    member = null;
    type = null;
    return false;
  }

  protected static MemberInfo GetMember(Type targetType, string memberName)
  {
    (MemberInfo member, _) = GetMemberAndType(targetType, memberName);
    return member;
  }

  protected static (MemberInfo Member, Type Type) GetMemberAndType(Type targetType, string memberName)
  {
    MemberInfo[] members = targetType.GetMember(
      name: memberName,
      type: MemberTypes.Field | MemberTypes.Property,
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

    if (members.Length == 0)
      throw new ModelConfigurationException($"Could not find a property or a field named '{memberName}'.");

    if (members.Length > 1)
      throw new ModelConfigurationException($"Ambiguous member name '{memberName}'.");

    return members[0] switch
    {
      PropertyInfo property => (property, property.PropertyType),
      FieldInfo field       => (field, field.FieldType),
      _                     => throw Errors.Unreachable,
    };
  }

  protected static IEnumerable<Name> GetKeyNames(Expression body)
  {
    return GetKeyNamesCore(body, false);
  }

  protected static void CheckKeyArity(int arity)
  {
    if (arity > 4)
      throw new NotSupportedException("Primary keys with more than 4 components are not supported.");
  }

  private static IEnumerable<Name> GetKeyNamesCore(Expression expression, bool throwOnNewExpression)
  {
    MemberInfo? member;
    switch (expression)
    {
      case MemberExpression memberExpression:
        if (!TryGetMemberAndTypeFromExpression(memberExpression, out member, out _))
          throw InvalidExpression();

        yield return member.Name;
        break;

      case NewExpression newExpression:
        if (throwOnNewExpression || !newExpression.Type.IsAnonymousType())
          throw InvalidExpression();

        foreach (Expression argument in newExpression.Arguments)
        {
          if (!TryGetMemberAndTypeFromExpression(argument, out member, out _))
            throw InvalidExpression();

          int arity = 0;
          foreach (Name name in GetKeyNamesCore(argument, true))
          {
            arity++;
            CheckKeyArity(arity);
            yield return name;
          }
        }
        break;

      case MethodCallExpression methodCallExpression:
        if (methodCallExpression.Method != ModelConfiguration.s_hiddenColumnMethod || methodCallExpression.Arguments[0] is not ConstantExpression columnNameExpression)
          throw InvalidExpression();

        yield return new ColumnName((string)columnNameExpression.Value);
        break;

      default:
        throw InvalidExpression();
    }

    static Exception InvalidExpression() => new ModelConfigurationException("The expression must be 'x => x.Key' for simple keys or 'x => new { x.Key1, ..., x.KeyN }' for composite keys.");
  }
}
