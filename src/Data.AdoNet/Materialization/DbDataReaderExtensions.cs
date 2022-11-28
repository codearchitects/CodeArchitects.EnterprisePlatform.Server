using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal static class DbDataReaderExtensions
{
  public static bool? GetNullableBoolean(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetBoolean(ordinal);
  }

  public static byte? GetNullableByte(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetByte(ordinal);
  }

  public static char? GetNullableChar(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetChar(ordinal);
  }

  public static DateTime? GetNullableDateTime(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetDateTime(ordinal);
  }

  public static decimal? GetNullableDecimal(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetDecimal(ordinal);
  }

  public static double? GetNullableDouble(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetDouble(ordinal);
  }

  public static float? GetNullableFloat(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetFloat(ordinal);
  }

  public static Guid? GetNullableGuid(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetGuid(ordinal);
  }

  public static short? GetNullableInt16(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetInt16(ordinal);
  }

  public static int? GetNullableInt32(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetInt32(ordinal);
  }

  public static long? GetNullableInt64(this DbDataReader dataReader, int ordinal)
  {
    if (dataReader.IsDBNull(ordinal))
      return null;

    return dataReader.GetInt64(ordinal);
  }
}
