using CodeArchitects.Platform.Common.Exceptions;

namespace CodeArchitects.Platform.Common.Expressions;

internal partial class ExpressionEvaluator
{
  private static object Add(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     + (int)right!,
      TypeCode.UInt32  => (uint)left!    + (uint)right!,
      TypeCode.Int64   => (long)left!    + (long)right!,
      TypeCode.UInt64  => (ulong)left!   + (ulong)right!,
      TypeCode.Single  => (float)left!   + (float)right!,
      TypeCode.Double  => (double)left!  + (double)right!,
      TypeCode.Decimal => (decimal)left! + (decimal)right!,
      TypeCode.String  => (string)left!  + (string)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object Subtract(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     - (int)right!,
      TypeCode.UInt32  => (uint)left!    - (uint)right!,
      TypeCode.Int64   => (long)left!    - (long)right!,
      TypeCode.UInt64  => (ulong)left!   - (ulong)right!,
      TypeCode.Single  => (float)left!   - (float)right!,
      TypeCode.Double  => (double)left!  - (double)right!,
      TypeCode.Decimal => (decimal)left! - (decimal)right!,
      _ => throw Errors.Unreachable
    };
  }

  private static object Multiply(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     * (int)right!,
      TypeCode.UInt32  => (uint)left!    * (uint)right!,
      TypeCode.Int64   => (long)left!    * (long)right!,
      TypeCode.UInt64  => (ulong)left!   * (ulong)right!,
      TypeCode.Single  => (float)left!   * (float)right!,
      TypeCode.Double  => (double)left!  * (double)right!,
      TypeCode.Decimal => (decimal)left! * (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object Divide(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     * (int)right!,
      TypeCode.UInt32  => (uint)left!    * (uint)right!,
      TypeCode.Int64   => (long)left!    * (long)right!,
      TypeCode.UInt64  => (ulong)left!   * (ulong)right!,
      TypeCode.Single  => (float)left!   * (float)right!,
      TypeCode.Double  => (double)left!  * (double)right!,
      TypeCode.Decimal => (decimal)left! * (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object Modulo(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     % (int)right!,
      TypeCode.UInt32  => (uint)left!    % (uint)right!,
      TypeCode.Int64   => (long)left!    % (long)right!,
      TypeCode.UInt64  => (ulong)left!   % (ulong)right!,
      TypeCode.Single  => (float)left!   % (float)right!,
      TypeCode.Double  => (double)left!  % (double)right!,
      TypeCode.Decimal => (decimal)left! % (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object LeftShift(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!   << (int)right!,
      TypeCode.UInt32  => (uint)left!  << (int)right!,
      TypeCode.Int64   => (long)left!  << (int)right!,
      TypeCode.UInt64  => (ulong)left! << (int)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object RightShift(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!   >> (int)right!,
      TypeCode.UInt32  => (uint)left!  >> (int)right!,
      TypeCode.Int64   => (long)left!  >> (int)right!,
      TypeCode.UInt64  => (ulong)left! >> (int)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object And(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Boolean => (bool)left!  & (bool)right!,
      TypeCode.Int32   => (int)left!   & (int)right!,
      TypeCode.UInt32  => (uint)left!  & (uint)right!,
      TypeCode.Int64   => (long)left!  & (long)right!,
      TypeCode.UInt64  => (ulong)left! & (ulong)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object Or(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Boolean => (bool)left!  | (bool)right!,
      TypeCode.Int32   => (int)left!   | (int)right!,
      TypeCode.UInt32  => (uint)left!  | (uint)right!,
      TypeCode.Int64   => (long)left!  | (long)right!,
      TypeCode.UInt64  => (ulong)left! | (ulong)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object ExclusiveOr(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Boolean => (bool)left!  ^ (bool)right!,
      TypeCode.Int32   => (int)left!   ^ (int)right!,
      TypeCode.UInt32  => (uint)left!  ^ (uint)right!,
      TypeCode.Int64   => (long)left!  ^ (long)right!,
      TypeCode.UInt64  => (ulong)left! ^ (ulong)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object AndAlso(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Boolean => (bool)left! && (bool)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object OrElse(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Boolean => (bool)left! || (bool)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object Equal(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     == (int)right!,
      TypeCode.UInt32  => (uint)left!    == (uint)right!,
      TypeCode.Int64   => (long)left!    == (long)right!,
      TypeCode.UInt64  => (ulong)left!   == (ulong)right!,
      TypeCode.Single  => (float)left!   == (float)right!,
      TypeCode.Double  => (double)left!  == (double)right!,
      TypeCode.Decimal => (decimal)left! == (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object NotEqual(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     != (int)right!,
      TypeCode.UInt32  => (uint)left!    != (uint)right!,
      TypeCode.Int64   => (long)left!    != (long)right!,
      TypeCode.UInt64  => (ulong)left!   != (ulong)right!,
      TypeCode.Single  => (float)left!   != (float)right!,
      TypeCode.Double  => (double)left!  != (double)right!,
      TypeCode.Decimal => (decimal)left! != (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object LessThan(object? left, object? right)
  {    
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     < (int)right!,
      TypeCode.UInt32  => (uint)left!    < (uint)right!,
      TypeCode.Int64   => (long)left!    < (long)right!,
      TypeCode.UInt64  => (ulong)left!   < (ulong)right!,
      TypeCode.Single  => (float)left!   < (float)right!,
      TypeCode.Double  => (double)left!  < (double)right!,
      TypeCode.Decimal => (decimal)left! < (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object LessThanOrEqual(object? left, object? right)
  {    
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     <= (int)right!,
      TypeCode.UInt32  => (uint)left!    <= (uint)right!,
      TypeCode.Int64   => (long)left!    <= (long)right!,
      TypeCode.UInt64  => (ulong)left!   <= (ulong)right!,
      TypeCode.Single  => (float)left!   <= (float)right!,
      TypeCode.Double  => (double)left!  <= (double)right!,
      TypeCode.Decimal => (decimal)left! <= (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object GreaterThanOrEqual(object? left, object? right)
  {    
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     >= (int)right!,
      TypeCode.UInt32  => (uint)left!    >= (uint)right!,
      TypeCode.Int64   => (long)left!    >= (long)right!,
      TypeCode.UInt64  => (ulong)left!   >= (ulong)right!,
      TypeCode.Single  => (float)left!   >= (float)right!,
      TypeCode.Double  => (double)left!  >= (double)right!,
      TypeCode.Decimal => (decimal)left! >= (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }

  private static object GreaterThan(object? left, object? right)
  {    
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Int32   => (int)left!     > (int)right!,
      TypeCode.UInt32  => (uint)left!    > (uint)right!,
      TypeCode.Int64   => (long)left!    > (long)right!,
      TypeCode.UInt64  => (ulong)left!   > (ulong)right!,
      TypeCode.Single  => (float)left!   > (float)right!,
      TypeCode.Double  => (double)left!  > (double)right!,
      TypeCode.Decimal => (decimal)left! > (decimal)right!,
      _                => throw Errors.Unreachable
    };
  }
}
