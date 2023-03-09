using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Common.Expressions;

/// <summary>
/// Exception thrown when an expression could not be evaluated dynamically.
/// </summary>
[Serializable]
public sealed class ExpressionEvaluationException : Exception
{
  /// <summary>
  /// Creates a new <see cref="ExpressionEvaluationException"/> providing the expression that could not be evaluated.
  /// </summary>
  /// <param name="expression">The expression.</param>
  internal ExpressionEvaluationException(Expression expression)
    : base($"Could not evaluate expression '{expression}'.")
  {
    Expression = expression;
  }

  /// <summary>
  /// Creates a new <see cref="ExpressionEvaluationException"/> providing the expression that could not be evaluated and the exception that caused the error.
  /// </summary>
  /// <param name="expression">The expression.</param>
  /// <param name="inner">The exception that caused the error.</param>
  internal ExpressionEvaluationException(Expression expression, Exception? inner)
    : base($"Could not evaluate expression '{expression}'.", inner)
  {
    Expression = expression;
  }

  private ExpressionEvaluationException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    Expression = new DeserializedExpression(info.GetString(nameof(Expression)) ?? string.Empty);
  }

  /// <summary>
  /// The expression that could not be evaluated.
  /// </summary>
  public Expression Expression { get; }

  public override void GetObjectData(SerializationInfo info, StreamingContext context)
  {
    info.AddValue(nameof(Expression), Expression.ToString());
    base.GetObjectData(info, context);
  }

  private sealed class DeserializedExpression : Expression
  {
    private readonly string _deserialized;

    public DeserializedExpression(string deserialized)
    {
      _deserialized = deserialized;
    }

    public override string ToString() => _deserialized;
  }
}
