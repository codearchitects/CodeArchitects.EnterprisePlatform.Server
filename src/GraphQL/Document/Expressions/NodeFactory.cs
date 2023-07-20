using CodeArchitects.Platform.Common.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal static class NodeFactory
{
  public static object? CreateValue(INodeRoot root, Expression expression)
  {
    return expression switch
    {
      NewExpression newExpression when newExpression.Type.IsAnonymousType() => new AnonymousObjectValueNode(root, newExpression),
      NewArrayExpression newArray                                           => new AnonymousListValueNode(root, newArray),
      _                                                                     => CreateValue(root, ExpressionEvaluator.Evaluate(expression))
    };
  }

  public static object? CreateValue(INodeRoot root, object? @object)
  {
    if (Convert.GetTypeCode(@object) is TypeCode.Object)
    {
      if (@object is IEnumerable values)
        return new ListValueNode(root, values.GetEnumerator());

      if (root.Context.TryGetObjectType(@object!.GetType(), out IObjectType? objectType))
        return new ObjectValueNode(root, objectType, @object);
    }

    return @object;
  }

  public static IArgumentNode CreateArgument(INodeRoot root, MethodCallExpression withArgumentCall)
  {
    ReadOnlyCollection<Expression> arguments = withArgumentCall.Arguments;

    switch (arguments.Count)
    {
      case 1:
        return CreateFromVariable(root, null, arguments[0]);
      
      case 2:
        string name = ExpressionEvaluator.Evaluate<string>(arguments[0]);
        return typeof(LambdaExpression).IsAssignableFrom(arguments[1].Type)
          ? CreateFromVariable(root, name, arguments[1])
          : CreateFromNameAndValue(root, name, arguments[1]);
      
      default:
        throw new ExpressionEvaluationException(withArgumentCall);
    }

    static IArgumentNode CreateFromNameAndValue(INodeRoot root, string name, Expression expression)
    {
      object? value = CreateValue(root, expression);

      return new ArgumentNode(name, value);
    }

    static VariableArgumentNode CreateFromVariable(INodeRoot root, string? name, Expression variableExpression)
    {
      LambdaExpression variableLambda = variableExpression.EvaluateAsLambda();

      if (variableLambda.Body is not MemberExpression variableMember)
        throw new ExpressionEvaluationException(variableLambda);

      if (!ReferenceEquals(variableMember.Expression, variableLambda.Parameters[0]))
        throw new ExpressionEvaluationException(variableLambda);

      if (variableMember.Member is not PropertyInfo property)
        throw new ExpressionEvaluationException(variableLambda);

      IVariable variable = root.Context.GetVariable(property);
      return new VariableArgumentNode(variable, name);
    }
  }

  public static IDirectiveNode CreateDirective(INodeRoot root, MethodCallExpression withDirectiveCall)
  {
    ReadOnlyCollection<Expression> arguments = withDirectiveCall.Arguments;

    if (arguments.Count == 0)
      throw new ExpressionEvaluationException(withDirectiveCall);

    string name = ExpressionEvaluator.Evaluate<string>(arguments[0]);
    if (arguments.Count == 1)
      return new SimpleDirectiveNode(name);

    LambdaExpression directiveExpression = arguments[1].EvaluateAsLambda();
    return new DirectiveNode(root, name, directiveExpression.Body);
  }

  public static ISelectionSetNode CreateSimpleSelectionSet(INodeRoot root, MethodCallExpression withSelectionCall)
  {
    ReadOnlyCollection<Expression> arguments = withSelectionCall.Arguments;

    if (arguments.Count < 1)
      throw new ExpressionEvaluationException(withSelectionCall);

    LambdaExpression selectionSet = arguments[0].EvaluateAsLambda();
    return selectionSet.Body switch
    {
      MemberInitExpression memberInit                     => new NamedSimpleSelectionSetNode(root, selectionSet.Parameters[0], memberInit),
      NewExpression @new when @new.Type.IsAnonymousType() => new AnonymousSimpleSelectionSetNode(root, selectionSet.Parameters[0], @new),
      _                                                   => throw new ExpressionEvaluationException(selectionSet)
    };
  }

  public static ISelectionSetNode CreateSelectionSet(INodeRoot root, MethodCallExpression withSelectionSetCall)
  {
    ReadOnlyCollection<Expression> arguments = withSelectionSetCall.Arguments;

    if (arguments.Count < 1)
      throw new ExpressionEvaluationException(withSelectionSetCall);

    LambdaExpression expansion = arguments[0].EvaluateAsLambda();
    return new SelectionSetNode(root, expansion.Body);
  }

  public static IFieldNode CreateField(INodeRoot root, Expression field, string memberName, Expression expression)
  {
    return expression switch
    {
      MemberExpression member         => CreateMemberField(root, field, memberName, member),
      MethodCallExpression methodCall => methodCall.Method.Name switch
      {
        MethodName.Field              => CreateRootField(root, field, memberName, methodCall),
        MethodName.SelectRef or
        MethodName.SelectCol          => CreateSelectField(root, field, memberName, methodCall),
        MethodName.ExpandRef or
        MethodName.ExpandCol          => CreateExpandField(root, field, memberName, methodCall),
        _                             => throw new ExpressionEvaluationException(methodCall)
      },
      _                               => throw new ExpressionEvaluationException(expression)
    };
  }

  public static IFragmentSpreadNode CreateFragmentSpread(INodeRoot root, MethodCallExpression withFragmentSpreadCall)
  {
    ReadOnlyCollection<Expression> arguments = withFragmentSpreadCall.Arguments;

    if (arguments.Count < 1)
      throw new ExpressionEvaluationException(withFragmentSpreadCall);

    GraphFragment fragment = ExpressionEvaluator.Evaluate<GraphFragment>(arguments[0]);
    root.ReportFragment(fragment);

    if (arguments.Count == 1)
      return new SimpleFragmentSpreadNode(fragment);

    LambdaExpression expansion = arguments[1].EvaluateAsLambda();
    return new FragmentSpreadNode(root, fragment, expansion.Body);
  }

  public static IInlineFragmentNode CreateInlineFragment(INodeRoot root, MethodCallExpression withInlineFragmentCall)
  {
    ReadOnlyCollection<Expression> arguments = withInlineFragmentCall.Arguments;

    if (arguments.Count < 1)
      throw new ExpressionEvaluationException(withInlineFragmentCall);

    MethodInfo withInlineFragmentMethod = withInlineFragmentCall.Method;
    LambdaExpression expansion = arguments[0].EvaluateAsLambda();

    if (!withInlineFragmentMethod.IsGenericMethod)
      return new UntypedInlineFragmentNode(root, expansion.Body);

    if (!root.Context.TryGetObjectType(withInlineFragmentMethod.GetGenericArguments()[0], out IObjectType? fragmentType))
      throw new ExpressionEvaluationException(withInlineFragmentCall);

    return new TypedInlineFragmentNode(root, expansion.Body, fragmentType);
  }

  private static IFieldNode CreateSelectField(INodeRoot root, Expression field, string memberName, MethodCallExpression selectCall)
  {
    ReadOnlyCollection<Expression> arguments = selectCall.Arguments;

    if (arguments.Count < 2)
      throw new ExpressionEvaluationException(selectCall);

    Expression source = arguments[0];
    LambdaExpression selection = arguments[1].EvaluateAsLambda();

    string fieldName = GetFieldName(field, source);
    string? alias = GetAlias(memberName, fieldName);

    return new FieldNode(root, alias, fieldName, selection);
  }

  private static IFieldNode CreateExpandField(INodeRoot root, Expression field, string memberName, MethodCallExpression expandCall)
  {
    ReadOnlyCollection<Expression> arguments = expandCall.Arguments;

    if (arguments.Count < 2)
      throw new ExpressionEvaluationException(expandCall);

    Expression source = arguments[0];
    LambdaExpression expansion = arguments[1].EvaluateAsLambda();

    string fieldName = GetFieldName(field, source);
    string? alias = GetAlias(memberName, fieldName);

    return new ExpandFieldNode(root, alias, fieldName, expansion.Body);
  }

  private static IFieldNode CreateMemberField(INodeRoot root, Expression field, string memberName, MemberExpression member)
  {
    if (!IsFieldMember(member, field))
      throw new ExpressionEvaluationException(member);

    string fieldName = member.Member.Name;
    string? alias = GetAlias(memberName, fieldName);

    if (root.Context.TryGetDefaultSelection(member.Type, out LambdaExpression? defaultSelection))
      return new FieldNode(root, alias, fieldName, defaultSelection);

    return new SimpleFieldNode(alias, fieldName);
  }

  private static IFieldNode CreateRootField(INodeRoot root, Expression rootExpression, string memberName, MethodCallExpression fieldCall)
  {
    if (!ReferenceEquals(fieldCall.Object, rootExpression))
      throw new ExpressionEvaluationException(fieldCall);

    string fieldName = ExpressionEvaluator.Evaluate<string>(fieldCall.Arguments[0]);
    string? alias = GetAlias(memberName, fieldName);

    if (root.Context.TryGetDefaultSelection(fieldCall.Type, out LambdaExpression? defaultSelection))
      return new FieldNode(root, alias, fieldName, defaultSelection);

    return new SimpleFieldNode(alias, fieldName);
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
    return methodCall.Method.Name == MethodName.Field && ReferenceEquals(methodCall.Object, root);
  }
}
