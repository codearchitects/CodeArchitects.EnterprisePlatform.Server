using CodeArchitects.Platform.Common.Exceptions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Common.Expressions;

internal partial class ExpressionEvaluator : ExpressionVisitor
{
  private static readonly ExpressionEvaluator s_instance = new();

  protected ExpressionEvaluator() { }

  protected override Optional<object?> VisitBinary(BinaryExpression node)
  {
    object? left = Visit(node.Left);
    object? right = Visit(node.Right);

    if (node.Method is { } method)
    {
      return method.Invoke(null, new[] { left, right });
    }

    return node.NodeType switch
    {
      ExpressionType.Add                => Add(left, right),
      ExpressionType.Subtract           => Subtract(left, right),
      ExpressionType.Multiply           => Multiply(left, right),
      ExpressionType.Divide             => Divide(left, right),
      ExpressionType.Modulo             => Modulo(left, right),
      ExpressionType.LeftShift          => LeftShift(left, right),
      ExpressionType.RightShift         => RightShift(left, right),
      ExpressionType.And                => And(left, right),
      ExpressionType.Or                 => Or(left, right),
      ExpressionType.ExclusiveOr        => ExclusiveOr(left, right),
      ExpressionType.AndAlso            => AndAlso(left, right),
      ExpressionType.OrElse             => OrElse(left, right),
      ExpressionType.Equal              => Equal(left, right),
      ExpressionType.NotEqual           => NotEqual(left, right),
      ExpressionType.LessThan           => LessThan(left, right),
      ExpressionType.LessThanOrEqual    => LessThanOrEqual(left, right),
      ExpressionType.GreaterThanOrEqual => GreaterThanOrEqual(left, right),
      ExpressionType.GreaterThan        => GreaterThan(left, right),
      _                                 => Errors.Unreachable
    };
  }

  protected override Optional<object?> VisitConditional(ConditionalExpression node)
  {
    bool test = (bool)Visit(node.Test)!;

    return Visit(test ? node.IfTrue : node.IfFalse);
  }

  protected override Optional<object?> VisitConstant(ConstantExpression node)
  {
    return node.Value;
  }

  protected override Optional<object?> VisitDefault(DefaultExpression node)
  {
    return node.Type.IsValueType ? Activator.CreateInstance(node.Type) : null;
  }

  protected override Optional<object?> VisitLambda<T>(Expression<T> node)
  {
    return node.Compile();
  }

  protected override Optional<object?> VisitListInit(ListInitExpression node)
  {
    object instance = Visit(node.NewExpression)!;
    foreach (ElementInit initializer in node.Initializers)
    {
      object?[] arguments = initializer.Arguments.Map(Visit);
      initializer.AddMethod.Invoke(instance, arguments);
    }

    return instance;
  }

  protected override Optional<object?> VisitMember(MemberExpression node)
  {
    object? instance = Visit(node.Expression);

    return node.Member.GetValue(instance);
  }

  protected override Optional<object?> VisitMemberInit(MemberInitExpression node)
  {
    object instance = Visit(node.NewExpression)!;
    VisitBindings(instance, node.Bindings);

    return instance;
  }

  protected override Optional<object?> VisitMethodCall(MethodCallExpression node)
  {
    object? @object = Visit(node.Object);
    object?[] arguments = node.Arguments.Map(Visit);

    return node.Method.Invoke(@object, arguments);
  }

  protected override Optional<object?> VisitNew(NewExpression node)
  {
    object?[] arguments = node.Arguments.Map(Visit);

    return node.Constructor!.Invoke(arguments);
  }

  protected override Optional<object?> VisitNewArray(NewArrayExpression node)
  {
    int length = node.Expressions.Count;
    Array array = Array.CreateInstance(node.Type.GetElementType()!, length);

    for (int i = 0; i < length; i++)
    {
      object? element = Visit(node.Expressions[i]);
      array.SetValue(element, i);
    }

    return array;
  }

  protected override Optional<object?> VisitParameter(ParameterExpression node, ReadOnlyCollection<ParameterExpression> parameters, object?[] arguments)
  {
    int index = parameters.IndexOf(node);
  
    if (index == -1)
      throw new InvalidOperationException($"Found a parameter which is not part of the lambda (parameter '{node.Name}').");
  
    return arguments[index];
  }

  protected override Optional<object?> VisitUnary(UnaryExpression node)
  {
    object? operand = Visit(node.Operand);

    if (node.Method is { } method)
    {
      return method.Invoke(null, new[] { operand });
    }

    return node.NodeType switch
    {
      ExpressionType.Increment      => Increment(operand),
      ExpressionType.Decrement      => Decrement(operand),
      ExpressionType.OnesComplement => OnesComplement(operand),
      ExpressionType.Not            => Not(operand),
      ExpressionType.UnaryPlus      => UnaryPlus(operand),
      ExpressionType.Negate         => Negate(operand),
      ExpressionType.Convert        => Convert(operand, node.Type),
      ExpressionType.Quote          => operand,
      _                             => Errors.Unreachable
    };
  }

  private void VisitBindings(object instance, ReadOnlyCollection<MemberBinding> bindings)
  {
    foreach (MemberBinding binding in bindings)
    {
      switch (binding)
      {
        case MemberAssignment assignment:
          object? value = Visit(assignment.Expression);
          binding.Member.SetValue(instance, value);
          break;

        case MemberMemberBinding memberBinding:
          object? memberValue = memberBinding.Member.GetValue(instance);
          VisitBindings(memberValue!, memberBinding.Bindings);
          break;

        case MemberListBinding listBinding:
          object? listValue = listBinding.Member.GetValue(instance);
          foreach (ElementInit initializer in listBinding.Initializers)
          {
            object?[] arguments = initializer.Arguments.Map(Visit);
            initializer.AddMethod.Invoke(listValue, arguments);
          }
          break;
      }
    }
  }


  public static T Evaluate<T>(Expression expression, ReadOnlyCollection<ParameterExpression>? parameters = null, object?[]? arguments = null)
  {
    object? result = s_instance.Execute(expression, parameters, arguments);
    Debug.Assert(result is T);

    return (T)result!;
  }

  public static object? Evaluate(Expression expression, ReadOnlyCollection<ParameterExpression>? parameters = null, object?[]? arguments = null)
  {
    return s_instance.Execute(expression, parameters, arguments);
  }
}
