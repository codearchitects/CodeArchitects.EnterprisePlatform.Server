using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class IdentityCacheKeyEqualityComparer : IEqualityComparer<IdentityCacheKey>
{
  public static readonly IdentityCacheKeyEqualityComparer Instance = new();

  private IdentityCacheKeyEqualityComparer() { }

  public bool Equals(IdentityCacheKey x, IdentityCacheKey y)
  {
    if (x.Model != y.Model)
      return false;

    if (x.Key.GetType() == typeof(byte[]))
    {
      Debug.Assert(y.Key.GetType() == typeof(byte[]), "Got two keys of different type belonging to the same entity model.");

      byte[] xBytes = (byte[])x.Key;
      byte[] yBytes = (byte[])y.Key;

      return xBytes.SequenceEqual(yBytes);
    }

    return x.Key.Equals(y.Key);
  }

  public int GetHashCode(IdentityCacheKey obj)
  {
    const int multiplier = -1521134295;

    int hashCode = obj.Model.GetHashCode();

    if (obj.Key.GetType() == typeof(byte[]))
    {
      byte[] bytes = (byte[])obj.Key;
      int processedBytes = 0;
      while (processedBytes + 4 <= bytes.Length)
      {
        hashCode = hashCode * multiplier + MemoryMarshal.AsRef<int>(bytes.AsSpan(processedBytes, 4));
        processedBytes += 4;
      }

      if (processedBytes != bytes.Length)
      {
        hashCode = hashCode * multiplier + MemoryMarshal.AsRef<int>(bytes.AsSpan(bytes.Length - 4, 4));
      }

      return hashCode;
    }

    return hashCode * multiplier + obj.Key.GetHashCode();
  }
}
