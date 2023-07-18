using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Common.Expressions;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using ExpressionVisitor = System.Linq.Expressions.ExpressionVisitor;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class StreamingNode : ExpressionVisitor
{
  private MethodCallExpression? _currentCall;

  protected abstract Expression Expression { get; }

  private bool Done => ReferenceEquals(Expression, _currentCall);

  protected abstract object OnMethodCall(MethodCallExpression methodCall);

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  protected T GetCurrent<T>()
  {
    Debug.Assert(_currentCall is not null);
    return (T)OnMethodCall(_currentCall);
  }

  [MemberNotNullWhen(true, nameof(_currentCall))]
  protected bool MoveNext(string methodName)
  {
    if (Done)
      return false; // We are at the end of the method chain, our method was not called

    MethodCallExpression? previousCall = _currentCall;
    Visit(Expression);

    // At this point, _currentCall can be null only if there are no method calls in our expression, which would be something like x => x
    if (_currentCall?.Method.Name != methodName)
    {
      // We didn't find our method call and overshot: go back to the previous method call so we can advance when the next element is accessed
      _currentCall = previousCall;
      return false;
    }

    return true;
  }

  protected T GetNext<T>(string methodName)
    where T : class
  {
    if (MoveNext(methodName))
      return GetCurrent<T>();

    // If the method call is mandatory, we should have found it
    Debug.Fail($"'{methodName}' was not called inside the expression.");
    throw Errors.Unreachable;
  }

  protected bool TryGetNext<T>(string methodName, [NotNullWhen(true)] out T? next)
    where T : class
  {
    if (MoveNext(methodName))
    {
      next = GetCurrent<T>();
      return true;
    }

    // We are at the end of the method chain, our method was not called
    next = null;
    return false;
  }

  protected bool Peek(string methodName)
  {
    MethodCallExpression? previousCall = _currentCall;

    if (MoveNext(methodName))
    {
      _currentCall = previousCall;
      return true;
    }

    return false;
  }

  public sealed override Expression Visit(Expression node)
  {
    if (ReferenceEquals(node, _currentCall))
    {
      Debug.Assert(!Done);

      _currentCall = null; // We found the current method call, set it to null so it can be set to the next call in the VisitMethodCall method
      return node;
    }

    return base.Visit(node);
  }

  protected sealed override Expression VisitMethodCall(MethodCallExpression node)
  {
    Visit(node.Object); // Visit the previous node until CurrentCall is set

    if (_currentCall is not null) // If the visit of the previous node set CurrentCall then we found the next method call, now bubble up to the TryAdvance method
      return node;

    // If the previous node was a ParameterExpression or the current method call, then CurrentCall was set to null. Assign it the current node value
    _currentCall = node;

    return node;
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    Debug.Assert(_currentCall is null); // We are at the start of the method chain, so there should be no current method call
    return node;
  }

  #region Not valid

  // Our lambda should be a fluent method call chain

  [ExcludeFromCodeCoverage]
  protected override Expression VisitBinary(BinaryExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitBlock(BlockExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override CatchBlock VisitCatchBlock(CatchBlock node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitConditional(ConditionalExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitConstant(ConstantExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitDebugInfo(DebugInfoExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitDefault(DefaultExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitDynamic(DynamicExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override ElementInit VisitElementInit(ElementInit node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitExtension(Expression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitGoto(GotoExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitIndex(IndexExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitInvocation(InvocationExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitLabel(LabelExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override LabelTarget VisitLabelTarget(LabelTarget node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitLambda<T>(Expression<T> node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitListInit(ListInitExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitLoop(LoopExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitMember(MemberExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override MemberAssignment VisitMemberAssignment(MemberAssignment node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override MemberBinding VisitMemberBinding(MemberBinding node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitMemberInit(MemberInitExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override MemberListBinding VisitMemberListBinding(MemberListBinding node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitNew(NewExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitNewArray(NewArrayExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitSwitch(SwitchExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override SwitchCase VisitSwitchCase(SwitchCase node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitTry(TryExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitTypeBinary(TypeBinaryExpression node) => throw new ExpressionEvaluationException(Expression);

  [ExcludeFromCodeCoverage]
  protected override Expression VisitUnary(UnaryExpression node) => throw new ExpressionEvaluationException(Expression);

  #endregion
}
