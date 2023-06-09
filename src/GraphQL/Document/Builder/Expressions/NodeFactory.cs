using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal static class NodeFactory
{
  public static object? CreateValue(INodeContext context, Expression expression)
  {
    return expression switch
    {
      NewExpression newExpression when newExpression.Type.IsAnonymousType() => new AnonymousObjectValueNode(context, newExpression),
      NewArrayExpression newArray                                           => new AnonymousListValueNode(context, newArray),
      _                                                                     => CreateValue(context, ExpressionEvaluator.Evaluate(expression))
    };
  }

  public static object? CreateValue(INodeContext context, object? @object)
  {
    if (Convert.GetTypeCode(@object) is TypeCode.Object)
    {
      if (@object is IEnumerable values)
        return new ListValueNode(context, values.GetEnumerator());

      if (context.TryGetObjectType(@object!.GetType(), out IObjectType? objectType))
        return new ObjectValueNode(context, objectType, @object);
    }

    return @object;
  }

  public static IArgumentNode CreateArgument(INodeContext context, MethodCallExpression withArgumentCall)
  {
    ReadOnlyCollection<Expression> arguments = withArgumentCall.Arguments;

    switch (arguments.Count)
    {
      case 1:
        return CreateFromVariable(arguments[0]);
      
      case 2:
        string name = ExpressionEvaluator.Evaluate<string>(arguments[0]);
        return typeof(LambdaExpression).IsAssignableFrom(arguments[1].Type)
          ? CreateFromNameAndVariable(name, arguments[1])
          : CreateFromNameAndValue(context, name, arguments[1]);
      
      default:
        throw new ExpressionEvaluationException(withArgumentCall);
    }

    static IArgumentNode CreateFromVariable(Expression variable)
    {
      LambdaExpression variableLambda = variable.EvaluateAsLambda();
      MemberInfo member = GetVariableMember(variableLambda);

      return new ArgumentNode(member.Name, member);
    }

    static IArgumentNode CreateFromNameAndVariable(string name, Expression variable)
    {
      LambdaExpression variableLambda = variable.EvaluateAsLambda();
      MemberInfo member = GetVariableMember(variableLambda);

      return new ArgumentNode(name, member);
    }

    static IArgumentNode CreateFromNameAndValue(INodeContext context, string name, Expression expression)
    {
      object? value = CreateValue(context, expression);

      return new ArgumentNode(name, value);
    }

    static MemberInfo GetVariableMember(LambdaExpression variableLambda)
    {
      if (variableLambda.Body is not MemberExpression variableMember)
        throw new ExpressionEvaluationException(variableLambda);

      if (!ReferenceEquals(variableMember.Expression, variableLambda.Parameters[0]))
        throw new ExpressionEvaluationException(variableLambda);

      return variableMember.Member;
    }
  }

  public static IDirectiveNode CreateDirective(INodeContext context, MethodCallExpression withDirectiveCall)
  {
    ReadOnlyCollection<Expression> arguments = withDirectiveCall.Arguments;

    if (arguments.Count == 0)
      throw new ExpressionEvaluationException(withDirectiveCall);

    string name = ExpressionEvaluator.Evaluate<string>(arguments[0]);
    if (arguments.Count == 1)
      return new SimpleDirectiveNode(name);

    LambdaExpression directiveExpression = arguments[1].EvaluateAsLambda();
    return new DirectiveNode(context, name, directiveExpression.Body);
  }

  public static ISelectionSetNode CreateSelectionSet(INodeContext context, MethodCallExpression withSelectionCall)
  {
    ReadOnlyCollection<Expression> arguments = withSelectionCall.Arguments;

    if (arguments.Count < 1)
      throw new ExpressionEvaluationException(withSelectionCall);

    LambdaExpression selectionSet = arguments[0].EvaluateAsLambda();
    return selectionSet.Body switch
    {
      MemberInitExpression memberInit                     => new NamedSelectionSetNode(context, selectionSet.Parameters[0], memberInit),
      NewExpression @new when @new.Type.IsAnonymousType() => new AnonymousSelectionSetNode(context, selectionSet.Parameters[0], @new),
      _                                                   => throw new ExpressionEvaluationException(selectionSet)
    };
  }

  public static ISelectionNode CreateSelection(INodeContext context, Expression field, string memberName, Expression expression)
  {
    return expression switch
    {
      MemberExpression member         => CreateMemberSelection(context, field, memberName, member),
      MethodCallExpression methodCall => methodCall.Method.Name switch
      {
        MethodNames.Field             => CreateRootSelection(context, field, memberName, methodCall),
        MethodNames.SelectRef or
        MethodNames.SelectCol         => CreateSelectSelection(context, field, memberName, methodCall),
        MethodNames.ExpandRef or
        MethodNames.ExpandCol         => CreateExpandSelection(context, field, memberName, methodCall),
        _                             => throw new ExpressionEvaluationException(methodCall)
      },
      _                               => throw new ExpressionEvaluationException(expression)
    };
  }

  private static ISelectionNode CreateSelectSelection(INodeContext context, Expression field, string memberName, MethodCallExpression selectCall)
  {
    ReadOnlyCollection<Expression> arguments = selectCall.Arguments;

    if (arguments.Count < 2)
      throw new ExpressionEvaluationException(selectCall);

    Expression source = arguments[0];
    LambdaExpression selection = arguments[1].EvaluateAsLambda();

    string fieldName = GetFieldName(field, source);
    string? alias = GetAlias(memberName, fieldName);

    return new SelectionNode(context, alias, fieldName, selection);
  }

  private static ISelectionNode CreateExpandSelection(INodeContext context, Expression field, string memberName, MethodCallExpression expandCall)
  {
    ReadOnlyCollection<Expression> arguments = expandCall.Arguments;

    if (arguments.Count < 2)
      throw new ExpressionEvaluationException(expandCall);

    Expression source = arguments[0];
    LambdaExpression expansion = arguments[1].EvaluateAsLambda();

    string fieldName = GetFieldName(field, source);
    string? alias = GetAlias(memberName, fieldName);

    return new ExpandSelectionNode(context, alias, fieldName, expansion.Body);
  }

  private static ISelectionNode CreateMemberSelection(INodeContext context, Expression field, string memberName, MemberExpression member)
  {
    if (!IsFieldMember(member, field))
      throw new ExpressionEvaluationException(member);

    string fieldName = member.Member.Name;
    string? alias = GetAlias(memberName, fieldName);

    if (context.TryGetDefaultSelection(member.Type, out LambdaExpression? defaultSelection))
      return new SelectionNode(context, alias, fieldName, defaultSelection);

    return new SimpleSelectionNode(alias, fieldName);
  }

  private static ISelectionNode CreateRootSelection(INodeContext context, Expression root, string memberName, MethodCallExpression fieldCall)
  {
    if (!ReferenceEquals(fieldCall.Object, root))
      throw new ExpressionEvaluationException(fieldCall);

    string fieldName = ExpressionEvaluator.Evaluate<string>(fieldCall.Arguments[0]);
    string? alias = GetAlias(memberName, fieldName);

    if (context.TryGetDefaultSelection(fieldCall.Type, out LambdaExpression? defaultSelection))
      return new SelectionNode(context, alias, fieldName, defaultSelection);

    return new SimpleSelectionNode(alias, fieldName);
  }

  private static string GetFieldName(Expression field, Expression source)
  {
    return source switch
    {
      MethodCallExpression methodCall when IsFieldCall(methodCall, field) => ExpressionEvaluator.Evaluate<string>(methodCall.Arguments[0]),
      MemberExpression member when IsFieldMember(member, field)           => member.Member.Name,
      _                                                                   => throw new ExpressionEvaluationException(source)
    };
  }

  private static string? GetAlias(string memberName, string fieldName)
  {
    if (char.ToLower(memberName[0]) != char.ToLower(fieldName[0]))
      return memberName;

    if (!memberName.AsSpan()[1..].SequenceEqual(fieldName.AsSpan()[1..]))
      return memberName;

    return null;
  }

  private static bool IsFieldMember(MemberExpression member, Expression field)
  {
    return ReferenceEquals(member.Expression, field);
  }

  private static bool IsFieldCall(MethodCallExpression methodCall, Expression root)
  {
    return methodCall.Method.Name == MethodNames.Field && ReferenceEquals(methodCall.Object, root);
  }
}
