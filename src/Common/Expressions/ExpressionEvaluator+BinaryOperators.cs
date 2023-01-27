using CodeArchitects.Platform.Common.Exceptions;

namespace CodeArchitects.Platform.Common.Expressions;

internal partial class ExpressionEvaluator
{
  private object Add(object? left, object? right)
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

  private object Subtract(object? left, object? right)
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

  private object Multiply(object? left, object? right)
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

  private object Divide(object? left, object? right)
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

  private object Modulo(object? left, object? right)
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

  private object LeftShift(object? left, object? right)
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

  private object RightShift(object? left, object? right)
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

  private object And(object? left, object? right)
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

  private object Or(object? left, object? right)
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

  private object ExclusiveOr(object? left, object? right)
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

  private object AndAlso(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Boolean => (bool)left! && (bool)right!,
      _                => throw Errors.Unreachable
    };
  }

  private object OrElse(object? left, object? right)
  {
    return System.Convert.GetTypeCode(left) switch
    {
      TypeCode.Boolean => (bool)left! || (bool)right!,
      _                => throw Errors.Unreachable
    };
  }

  private object Equal(object? left, object? right)
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

  private object NotEqual(object? left, object? right)
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

  private object LessThan(object? left, object? right)
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

  private object LessThanOrEqual(object? left, object? right)
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

  private object GreaterThanOrEqual(object? left, object? right)
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

  private object GreaterThan(object? left, object? right)
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
