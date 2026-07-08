using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class LiteralValueNode : IIntValueNode, IFloatValueNode, IStringValueNode, IBooleanValueNode, INullValueNode, IEnumValueNode
{
  private readonly object? _value;

  public LiteralValueNode(object? value)
  {
    _value = value;
  }

  ValueNodeKind IValueNode.ValueKind
  {
    get
    {
      if (_value is null)
        return ValueNodeKind.NullValue;

      return Convert.GetTypeCode(_value) switch
      {
        >= TypeCode.SByte and <= TypeCode.UInt64   => _value is Enum ? ValueNodeKind.EnumValue : ValueNodeKind.IntValue,
        >= TypeCode.Single and <= TypeCode.Decimal => ValueNodeKind.FloatValue,
        TypeCode.String                            => ValueNodeKind.StringValue,
        TypeCode.Boolean                           => ValueNodeKind.BooleanValue,
        TypeCode.Empty or TypeCode.DBNull          => ValueNodeKind.NullValue,
        _                                          => throw new NotSupportedException($"Value of type '' not supported."),
      };
    }
  }

  int IIntValueNode.Value => Convert.ToInt32(_value);

  double IFloatValueNode.Value => Convert.ToDouble(_value);

  string IStringValueNode.Value => Convert.ToString(_value);

  bool IBooleanValueNode.Value => Convert.ToBoolean(_value);

  string IEnumValueNode.Value => Convert.ToString(_value);
}
