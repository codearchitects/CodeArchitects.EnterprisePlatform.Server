using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Common.Expressions;

internal abstract class ExpressionVisitor
{
  [ThreadStatic]
  private static InnerVisitor? t_instance;

  public virtual object? Execute(Expression expression, ReadOnlyCollection<ParameterExpression>? parameters = null, object?[]? arguments = null)
  {
    t_instance ??= new();

    ExpressionVisitor? previousVisitor = t_instance.Visitor;
    ReadOnlyCollection<ParameterExpression>? previousParameters = t_instance.Parameters;
    object?[]? previousArguments = t_instance.Arguments;
    Optional<object?> previousResult = t_instance.Result;

    t_instance.Visitor = this;
    t_instance.Parameters = parameters;
    t_instance.Arguments = arguments;
    t_instance.Result = default;

    try
    {
      return t_instance.Visit(expression);
    }
    catch (Exception ex)
    {
      throw new ExpressionEvaluationException(expression, ex);
    }
    finally
    {
      t_instance.Visitor = previousVisitor;
      t_instance.Parameters = previousParameters;
      t_instance.Arguments = previousArguments;
      t_instance.Result = previousResult;
    }
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected virtual object? Visit(Expression? expression)
  {
    Debug.Assert(t_instance is not null);
    return t_instance.Visit(expression);
  }

  protected virtual Optional<object?> VisitBinary(BinaryExpression node)
    => DefaultVisit(node, typeof(BinaryExpression));

  protected virtual Optional<object?> VisitBlock(BlockExpression node)
    => DefaultVisit(node, typeof(BlockExpression));

  protected virtual Optional<object?> VisitCatchBlock(CatchBlock node)
    => throw NotSupported(typeof(CatchBlock));

  protected virtual Optional<object?> VisitConditional(ConditionalExpression node)
    => DefaultVisit(node, typeof(ConditionalExpression));

  protected virtual Optional<object?> VisitConstant(ConstantExpression node)
    => DefaultVisit(node, typeof(ConstantExpression));

  protected virtual Optional<object?> VisitDebugInfo(DebugInfoExpression node)
    => DefaultVisit(node, typeof(DebugInfoExpression));

  protected virtual Optional<object?> VisitDefault(DefaultExpression node)
    => DefaultVisit(node, typeof(DefaultExpression));

  protected virtual Optional<object?> VisitDynamic(DynamicExpression node)
    => DefaultVisit(node, typeof(DynamicExpression));

  protected virtual Optional<object?> VisitElementInit(ElementInit node)
    => throw NotSupported(typeof(ElementInit));

  protected virtual Optional<object?> VisitExtension(Expression node)
    => DefaultVisit(node, node.GetType());

  protected virtual Optional<object?> VisitGoto(GotoExpression node)
    => DefaultVisit(node, typeof(GotoExpression));

  protected virtual Optional<object?> VisitIndex(IndexExpression node)
    => DefaultVisit(node, typeof(IndexExpression));

  protected virtual Optional<object?> VisitInvocation(InvocationExpression node)
    => DefaultVisit(node, typeof(InvocationExpression));
  
  protected virtual Optional<object?> VisitLabel(LabelExpression node)
    => DefaultVisit(node, typeof(LabelExpression));

  protected virtual Optional<object?> VisitLabelTarget(LabelTarget node)
    => throw NotSupported(typeof(LabelTarget));

  protected virtual Optional<object?> VisitLambda<T>(Expression<T> node)
    => DefaultVisit(node, typeof(LambdaExpression));
  
  protected virtual Optional<object?> VisitListInit(ListInitExpression node)
    => DefaultVisit(node, typeof(ListInitExpression));
  
  protected virtual Optional<object?> VisitLoop(LoopExpression node)
    => DefaultVisit(node, typeof(LoopExpression));
  
  protected virtual Optional<object?> VisitMember(MemberExpression node)
    => DefaultVisit(node, typeof(MemberExpression));
  
  protected virtual Optional<object?> VisitMemberAssignment(MemberAssignment node)
    => throw NotSupported(typeof(MemberAssignment));
  
  protected virtual Optional<object?> VisitMemberBinding(MemberBinding node)
    => throw NotSupported(typeof(MemberBinding));
  
  protected virtual Optional<object?> VisitMemberInit(MemberInitExpression node)
    => DefaultVisit(node, typeof(MemberInitExpression));
  
  protected virtual Optional<object?> VisitMemberListBinding(MemberListBinding node)
    => throw NotSupported(typeof(MemberListBinding));
  
  protected virtual Optional<object?> VisitMemberMemberBinding(MemberMemberBinding node)
    => throw NotSupported(typeof(MemberMemberBinding));
  
  protected virtual Optional<object?> VisitMethodCall(MethodCallExpression node)
    => DefaultVisit(node, typeof(MethodCallExpression));
  
  protected virtual Optional<object?> VisitNew(NewExpression node)
    => DefaultVisit(node, typeof(NewExpression));
  
  protected virtual Optional<object?> VisitNewArray(NewArrayExpression node)
    => DefaultVisit(node, typeof(NewArrayExpression));
  
  protected virtual Optional<object?> VisitParameter(ParameterExpression node, ReadOnlyCollection<ParameterExpression> parameters, object?[] arguments)
    => DefaultVisit(node, typeof(ParameterExpression));
  
  protected virtual Optional<object?> VisitRuntimeVariables(RuntimeVariablesExpression node)
    => DefaultVisit(node, typeof(RuntimeVariablesExpression));
  
  protected virtual Optional<object?> VisitSwitch(SwitchExpression node)
    => DefaultVisit(node, typeof(SwitchExpression));
  
  protected virtual Optional<object?> VisitSwitchCase(SwitchCase node)
    => throw NotSupported(typeof(SwitchCase));
  
  protected virtual Optional<object?> VisitTry(TryExpression node)
    => DefaultVisit(node, typeof(TryExpression));
  
  protected virtual Optional<object?> VisitTypeBinary(TypeBinaryExpression node)
    => DefaultVisit(node, typeof(TypeBinaryExpression));
  
  protected virtual Optional<object?> VisitUnary(UnaryExpression node)
    => DefaultVisit(node, typeof(UnaryExpression));

  protected virtual Optional<object?> DefaultVisit(Expression expression, Type type)
    => throw NotSupported(type);

  private static Exception NotSupported(Type type)
    => new NotSupportedException($"Expression node of type '{type.FullName}' is not supported.");

  private sealed class InnerVisitor : System.Linq.Expressions.ExpressionVisitor
  {
    public ExpressionVisitor? Visitor;
    public ReadOnlyCollection<ParameterExpression>? Parameters;
    public object?[]? Arguments;
    public Optional<object?> Result;

    protected override Expression VisitBinary(BinaryExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitBinary(node);
      return node;
    }

    protected override Expression VisitBlock(BlockExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitBlock(node);
      return node;
    }

    protected override CatchBlock VisitCatchBlock(CatchBlock node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitCatchBlock(node);
      return node;
    }

    protected override Expression VisitConditional(ConditionalExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitConditional(node);
      return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitConstant(node);
      return node;
    }

    protected override Expression VisitDebugInfo(DebugInfoExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitDebugInfo(node);
      return node;
    }

    protected override Expression VisitDefault(DefaultExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitDefault(node);
      return node;
    }

    protected override Expression VisitDynamic(DynamicExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitDynamic(node);
      return node;
    }

    protected override ElementInit VisitElementInit(ElementInit node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitElementInit(node);
      return node;
    }

    protected override Expression VisitExtension(Expression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitExtension(node);
      return node;
    }

    protected override Expression VisitGoto(GotoExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitGoto(node);
      return node;
    }

    protected override Expression VisitIndex(IndexExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitIndex(node);
      return node;
    }

    protected override Expression VisitInvocation(InvocationExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitInvocation(node);
      return node;
    }

    protected override Expression VisitLabel(LabelExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitLabel(node);
      return node;
    }

    [return: NotNullIfNotNull(nameof(node))]
    protected override LabelTarget? VisitLabelTarget(LabelTarget? node)
    {
      Debug.Assert(Visitor is not null);

      if (node is null)
        return null;

      Result = Visitor.VisitLabelTarget(node);
      return node;
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitLambda(node);
      return node;
    }

    protected override Expression VisitListInit(ListInitExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitListInit(node);
      return node;
    }

    protected override Expression VisitLoop(LoopExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitLoop(node);
      return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitMember(node);
      return node;
    }

    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitMemberAssignment(node);
      return node;
    }

    protected override MemberBinding VisitMemberBinding(MemberBinding node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitMemberBinding(node);
      return node;
    }

    protected override Expression VisitMemberInit(MemberInitExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitMemberInit(node);
      return node;
    }

    protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitMemberListBinding(node);
      return node;
    }

    protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitMemberMemberBinding(node);
      return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitMethodCall(node);
      return node;
    }

    protected override Expression VisitNew(NewExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitNew(node);
      return node;
    }

    protected override Expression VisitNewArray(NewArrayExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitNewArray(node);
      return node;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
      Debug.Assert(Visitor is not null);

      if (Parameters is null || Arguments is null)
        throw new InvalidOperationException($"Could not evaluate the arguments of the provided expression (parameter '{node.Name}').");

      Result = Visitor.VisitParameter(node, Parameters, Arguments);
      return node;
    }

    protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitRuntimeVariables(node);
      return node;
    }

    protected override Expression VisitSwitch(SwitchExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitSwitch(node);
      return node;
    }

    protected override SwitchCase VisitSwitchCase(SwitchCase node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitSwitchCase(node);
      return node;
    }

    protected override Expression VisitTry(TryExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitTry(node);
      return node;
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitTypeBinary(node);
      return node;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
      Debug.Assert(Visitor is not null);
      Result = Visitor.VisitUnary(node);
      return node;
    }

    public new object? Visit(Expression? expression)
    {
      Debug.Assert(!Result.HasValue);

      if (expression is null)
        return null;

      base.Visit(expression);

      Debug.Assert(Result.HasValue);

      object? result = Result.Value;
      Result = default;

      return result;
    }
  }
}
