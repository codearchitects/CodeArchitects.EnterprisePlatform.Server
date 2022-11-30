using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal static class DataRecordExtensions
{
  public static bool? GetNullableBoolean(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetBoolean(ordinal);
  }

  public static byte? GetNullableByte(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetByte(ordinal);
  }

  public static char? GetNullableChar(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetChar(ordinal);
  }

  public static DateTime? GetNullableDateTime(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetDateTime(ordinal);
  }

  public static decimal? GetNullableDecimal(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetDecimal(ordinal);
  }

  public static double? GetNullableDouble(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetDouble(ordinal);
  }

  public static float? GetNullableFloat(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetFloat(ordinal);
  }

  public static Guid? GetNullableGuid(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetGuid(ordinal);
  }

  public static short? GetNullableInt16(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetInt16(ordinal);
  }

  public static int? GetNullableInt32(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetInt32(ordinal);
  }

  public static long? GetNullableInt64(this IDataRecord record, int ordinal)
  {
    if (record.IsDBNull(ordinal))
      return null;

    return record.GetInt64(ordinal);
  }
}
