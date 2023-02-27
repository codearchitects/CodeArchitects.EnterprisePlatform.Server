using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Common.Expressions;

[Serializable]
internal class ExpressionEvaluationException : Exception
{
  public ExpressionEvaluationException(Expression expression)
    : base($"Could not evaluate expression '{expression}'.")
  {
    Expression = expression;
  }

  public ExpressionEvaluationException(Expression expression, Exception? inner)
    : base($"Could not evaluate expression '{expression}'.", inner)
  {
    Expression = expression;
  }

  protected ExpressionEvaluationException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
    Expression = new DeserializedExpression(info.GetString(nameof(Expression)) ?? string.Empty);
  }

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
