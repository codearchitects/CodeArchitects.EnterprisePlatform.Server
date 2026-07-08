namespace CodeArchitects.Platform.Common.Utils;

internal static class Rand // To have a thread-safe random number generator mechanism without .NET 6+
{
  private static readonly Random s_global = new();

  [ThreadStatic]
  private static Random? t_instance;

  public static Random Instance
  {
    get
    {
      if (t_instance is null)
      {
        int seed;
        lock (s_global)
        {
          seed = s_global.Next();
        }

        t_instance = new Random(seed);
      }

      return t_instance;
    }
  }
}
