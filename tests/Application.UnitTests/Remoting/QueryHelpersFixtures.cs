using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Application.Remoting;

internal static class QueryHelpersFixtures
{
  public class SimpleTypesOrArraysDataAttribute : DataAttribute
  {
    private static readonly Type[] s_simpleTypes = new[]
    {
      typeof(bool),
      typeof(byte),
      typeof(char),
      typeof(short),
      typeof(int),
      typeof(long),
      typeof(ushort),
      typeof(uint),
      typeof(ulong),
      typeof(float),
      typeof(double),
      typeof(decimal),
      typeof(string),
      typeof(Guid),
      typeof(DateTime),
      typeof(DateTimeOffset)
    };

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
      foreach (Type simpleType in s_simpleTypes)
      {
        yield return new[] { simpleType };
        yield return new[] { simpleType.MakeArrayType() };
        yield return new[] { typeof(IEnumerable<>).MakeGenericType(simpleType) };
        yield return new[] { typeof(List<>).MakeGenericType(simpleType) };
      }
    }
  }
}
