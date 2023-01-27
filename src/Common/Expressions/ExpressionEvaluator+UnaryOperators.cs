using CodeArchitects.Platform.Common.Exceptions;

namespace CodeArchitects.Platform.Common.Expressions;

internal partial class ExpressionEvaluator
{
  private object Increment(object? operand)
  {
    return System.Convert.GetTypeCode(operand) switch
    {
      TypeCode.Char    => (char)((char)operand! + 1),
      TypeCode.Byte    => (byte)((byte)operand! + 1),
      TypeCode.SByte   => (sbyte)((sbyte)operand! + 1),
      TypeCode.Int16   => (short)((short)operand! + 1),
      TypeCode.UInt16  => (ushort)((ushort)operand! + 1),
      TypeCode.Int32   => (int)operand! + 1,
      TypeCode.UInt32  => (uint)operand! + 1,
      TypeCode.Int64   => (long)operand! + 1,
      TypeCode.UInt64  => (ulong)operand! + 1,
      TypeCode.Single  => (float)operand! + 1.0f,
      TypeCode.Double  => (double)operand! + 1.0,
      TypeCode.Decimal => (decimal)operand! + 1.0m,
      _                => Errors.Unreachable
    };
  }

  private object Decrement(object? operand)
  {
    return System.Convert.GetTypeCode(operand) switch
    {
      TypeCode.Char    => (char)((char)operand! - 1),
      TypeCode.Byte    => (byte)((byte)operand! - 1),
      TypeCode.SByte   => (sbyte)((sbyte)operand! - 1),
      TypeCode.Int16   => (short)((short)operand! - 1),
      TypeCode.UInt16  => (ushort)((ushort)operand! - 1),
      TypeCode.Int32   => (int)operand! - 1,
      TypeCode.UInt32  => (uint)operand! - 1,
      TypeCode.Int64   => (long)operand! - 1,
      TypeCode.UInt64  => (ulong)operand! - 1,
      TypeCode.Single  => (float)operand! - 1.0f,
      TypeCode.Double  => (double)operand! - 1.0,
      TypeCode.Decimal => (decimal)operand! - 1.0m,
      _                => Errors.Unreachable
    };
  }

  private object OnesComplement(object? operand)
  {
    return System.Convert.GetTypeCode(operand) switch
    {
      TypeCode.Char    => ~(char)operand!,
      TypeCode.Byte    => ~(byte)operand!,
      TypeCode.SByte   => ~(sbyte)operand!,
      TypeCode.Int16   => ~(short)operand!,
      TypeCode.UInt16  => ~(ushort)operand!,
      TypeCode.Int32   => ~(int)operand!,
      TypeCode.UInt32  => ~(uint)operand!,
      TypeCode.Int64   => ~(long)operand!,
      _                => Errors.Unreachable
    };
  }

  private object Not(object? operand)
  {
    return System.Convert.GetTypeCode(operand) switch
    {
      TypeCode.Boolean => !(bool)operand!,
      _                => Errors.Unreachable
    };
  }

  private object UnaryPlus(object? operand)
  {
    return System.Convert.GetTypeCode(operand) switch
    {
      TypeCode.Char    => +(char)operand!,
      TypeCode.Byte    => +(byte)operand!,
      TypeCode.SByte   => +(sbyte)operand!,
      TypeCode.Int16   => +(short)operand!,
      TypeCode.UInt16  => +(ushort)operand!,
      TypeCode.Int32   => +(int)operand!,
      TypeCode.UInt32  => +(uint)operand!,
      TypeCode.Int64   => +(long)operand!,
      TypeCode.UInt64  => +(ulong)operand!,
      TypeCode.Single  => +(float)operand!,
      TypeCode.Double  => +(double)operand!,
      TypeCode.Decimal => +(decimal)operand!,
      _                => Errors.Unreachable
    };
  }

  private object Negate(object? operand)
  {
    return System.Convert.GetTypeCode(operand) switch
    {
      TypeCode.Char    => -(char)operand!,
      TypeCode.Byte    => -(byte)operand!,
      TypeCode.SByte   => -(sbyte)operand!,
      TypeCode.Int16   => -(short)operand!,
      TypeCode.UInt16  => -(ushort)operand!,
      TypeCode.Int32   => -(int)operand!,
      TypeCode.UInt32  => -(uint)operand!,
      TypeCode.Int64   => -(long)operand!,
      TypeCode.Single  => -(float)operand!,
      TypeCode.Double  => -(double)operand!,
      TypeCode.Decimal => -(decimal)operand!,
      _                => Errors.Unreachable
    };
  }

  private object? Convert(object? operand, Type type)
  {
    if (operand is null)
    {
      if (type.IsValueType)
        throw new NullReferenceException();

      return null;
    }

    if (!type.IsInstanceOfType(operand))
      throw new InvalidCastException($"Unable to cast object of type '{operand.GetType()}' to type '{type}'.");

    return operand;
  }
}
