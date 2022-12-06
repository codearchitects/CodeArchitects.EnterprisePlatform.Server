namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal static class Configuration
{
  public static readonly IReadOnlyCollection<Type> SupportedColumnTypes = new HashSet<Type>()
  {
    typeof(bool),
    typeof(byte),
    typeof(char),
    typeof(DateTime),
    typeof(decimal),
    typeof(double),
    typeof(float),
    typeof(Guid),
    typeof(short),
    typeof(int),
    typeof(long),
    typeof(string),
    typeof(bool?),
    typeof(byte?),
    typeof(char?),
    typeof(DateTime?),
    typeof(decimal?),
    typeof(double?),
    typeof(float?),
    typeof(Guid?),
    typeof(short?),
    typeof(int?),
    typeof(long?)
  };

  public static bool IsSupportedColumnType(Type type)
  {
    return SupportedColumnTypes.Contains(type);
  }
}
