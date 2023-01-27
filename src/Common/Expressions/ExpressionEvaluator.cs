using CodeArchitects.Platform.Common.Collections;
using CodeArchitects.Platform.Common.Exceptions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Common.Expressions;

internal partial class ExpressionEvaluator : ExpressionVisitor
{
  [ThreadStatic]
  private static readonly ExpressionEvaluator t_instance = new();

  private ReadOnlyCollection<ParameterExpression>? _parameters;
  private object?[]? _arguments;
  private Optional<object?> _value;

  protected override Expression VisitBinary(BinaryExpression node)
  {
    object? left = GetValue(node.Left);
    object? right = GetValue(node.Right);

    if (node.Method is { } method)
    {
      _value = method.Invoke(null, new[] { left, right });
      return node;
    }

    _value = node.NodeType switch
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
    return node;
  }

  protected override Expression VisitConditional(ConditionalExpression node)
  {
    bool test = (bool)GetValue(node.Test)!;

    _value = GetValue(test ? node.IfTrue : node.IfFalse);
    return node;
  }

  protected override Expression VisitConstant(ConstantExpression node)
  {
    _value = node.Value;
    return node;
  }

  protected override Expression VisitDefault(DefaultExpression node)
  {
    _value = node.Type.IsValueType ? Activator.CreateInstance(node.Type) : null;
    return node;
  }

  protected override Expression VisitLambda<T>(Expression<T> node)
  {
    _value = node.Compile();
    return node;
  }

  protected override Expression VisitListInit(ListInitExpression node)
  {
    object instance = GetValue(node.NewExpression)!;
    foreach (ElementInit initializer in node.Initializers)
    {
      object?[] arguments = initializer.Arguments.Map(GetValue);
      initializer.AddMethod.Invoke(instance, arguments);
    }

    _value = instance;
    return node;
  }

  protected override Expression VisitMember(MemberExpression node)
  {
    object? instance = GetValue(node.Expression);

    _value = node.Member.GetValue(instance);
    return node;
  }

  protected override Expression VisitMemberInit(MemberInitExpression node)
  {
    object instance = GetValue(node.NewExpression)!;
    VisitBindings(this, instance, node.Bindings);

    _value = instance;
    return node;

    static void VisitBindings(ExpressionEvaluator self, object instance, ReadOnlyCollection<MemberBinding> bindings)
    {
      foreach (MemberBinding binding in bindings)
      {
        switch (binding)
        {
          case MemberAssignment assignment:
            object? value = self.GetValue(assignment.Expression);
            binding.Member.SetValue(instance, value);
            break;

          case MemberMemberBinding memberBinding:
            object? memberValue = memberBinding.Member.GetValue(instance);
            VisitBindings(self, memberValue!, memberBinding.Bindings);
            break;

          case MemberListBinding listBinding:
            object? listValue = listBinding.Member.GetValue(instance);
            foreach (ElementInit initializer in listBinding.Initializers)
            {
              object?[] arguments = initializer.Arguments.Map(self.GetValue);
              initializer.AddMethod.Invoke(listValue, arguments);
            }
            break;
        }
      }
    }
  }

  protected override Expression VisitMethodCall(MethodCallExpression node)
  {
    object? @object = GetValue(node.Object);
    object?[] arguments = node.Arguments.Map(this, static (self, argument) => self.GetValue(argument));

    _value = node.Method.Invoke(@object, arguments);
    return node;
  }

  protected override Expression VisitNew(NewExpression node)
  {
    object?[] arguments = node.Arguments.Map(this, static (self, argument) => self.GetValue(argument));

    _value = node.Constructor!.Invoke(arguments);
    return node;
  }

  protected override Expression VisitNewArray(NewArrayExpression node)
  {
    int length = node.Expressions.Count;
    Array array = Array.CreateInstance(node.Type.GetElementType()!, length);

    for (int i = 0; i < length; i++)
    {
      object? element = GetValue(node.Expressions[i]);
      array.SetValue(element, i);
    }

    _value = array;
    return node;
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    if (_parameters is null || _arguments is null)
      throw new InvalidOperationException($"Could not evaluate the arguments of the provided expression (parameter '{node.Name}').");

    int index = _parameters.IndexOf(node);

    if (index == -1)
      throw new InvalidOperationException($"Found a parameter which is not part of the lambda (parameter '{node.Name}').");

    _value = _arguments[index];
    return node;
  }

  protected override Expression VisitUnary(UnaryExpression node)
  {
    object? operand = GetValue(node.Operand);

    if (node.Method is { } method)
    {
      _value = method.Invoke(null, new[] { operand });
      return node;
    }

    _value = node.NodeType switch
    {
      ExpressionType.Increment      => Increment(operand),
      ExpressionType.Decrement      => Decrement(operand),
      ExpressionType.OnesComplement => OnesComplement(operand),
      ExpressionType.Not            => Not(operand),
      ExpressionType.UnaryPlus      => UnaryPlus(operand),
      ExpressionType.Negate         => Negate(operand),
      ExpressionType.Convert        => Convert(operand, node.Type),
      _                             => Errors.Unreachable
    };
    return node;
  }


  private object? GetValue(Expression? expression)
  {
    Debug.Assert(!_value.HasValue);

    if (expression is null)
      return null;

    Visit(expression);

    Debug.Assert(_value.HasValue);

    object? result = _value.Value;
    _value = default;

    return result;
  }

  private void Init(ReadOnlyCollection<ParameterExpression>? parameters, object?[]? arguments)
  {
    _arguments = arguments;
    _parameters = parameters;
  }

  private void Reset()
  {
    _arguments = null;
    _parameters = null;
  }

  public static T Evaluate<T>(Expression expression, ReadOnlyCollection<ParameterExpression>? parameters = null, object?[]? arguments = null)
  {
    try
    {
      t_instance.Init(parameters, arguments);
      return (T)t_instance.GetValue(expression)!;
    }
    catch (Exception ex)
    {
      throw new ExpressionEvaluationException(expression, ex);
    }
    finally
    {
      t_instance.Reset();
    }
  }

  private static Exception NotSupported(Type type)
    => new NotSupportedException($"Expression node of type '{type.FullName}' is not supported.");

  #region Not supported

  protected override Expression VisitBlock(BlockExpression node)
    => throw NotSupported(typeof(BlockExpression));

  protected override CatchBlock VisitCatchBlock(CatchBlock node)
    => throw NotSupported(typeof(CatchBlock));

  protected override Expression VisitDebugInfo(DebugInfoExpression node)
    => throw NotSupported(typeof(DebugInfoExpression));

  protected override Expression VisitDynamic(DynamicExpression node)
    => throw NotSupported(typeof(DynamicExpression));

  protected override ElementInit VisitElementInit(ElementInit node)
    => throw NotSupported(typeof(ElementInit));

  protected override Expression VisitExtension(Expression node)
    => throw NotSupported(node.GetType());

  protected override Expression VisitGoto(GotoExpression node)
    => throw NotSupported(typeof(GotoExpression));

  protected override Expression VisitIndex(IndexExpression node)
    => throw NotSupported(typeof(IndexExpression));

  protected override Expression VisitInvocation(InvocationExpression node)
    => throw NotSupported(typeof(InvocationExpression));

  protected override Expression VisitLabel(LabelExpression node)
    => throw NotSupported(typeof(LabelExpression));

  protected override LabelTarget VisitLabelTarget(LabelTarget? node)
    => throw NotSupported(typeof(LabelTarget));

  protected override Expression VisitLoop(LoopExpression node)
    => throw NotSupported(typeof(LoopExpression));

  protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    => throw NotSupported(typeof(MemberAssignment));

  protected override MemberBinding VisitMemberBinding(MemberBinding node)
    => throw NotSupported(typeof(MemberBinding));

  protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
    => throw NotSupported(typeof(MemberListBinding));

  protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
    => throw NotSupported(typeof(MemberMemberBinding));

  protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
    => throw NotSupported(typeof(RuntimeVariablesExpression));

  protected override Expression VisitSwitch(SwitchExpression node)
    => throw NotSupported(typeof(SwitchExpression));

  protected override SwitchCase VisitSwitchCase(SwitchCase node)
    => throw NotSupported(typeof(SwitchCase));

  protected override Expression VisitTry(TryExpression node)
    => throw NotSupported(typeof(TryExpression));

  protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    => throw NotSupported(typeof(TypeBinaryExpression));

  #endregion
}
