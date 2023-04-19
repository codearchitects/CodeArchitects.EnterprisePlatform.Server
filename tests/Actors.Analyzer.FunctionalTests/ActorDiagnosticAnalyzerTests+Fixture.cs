using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Actors.Analyzer;

public partial class ActorDiagnosticAnalyzerTests
{
  public class CAEPACTR300ValidDictionaryDataAttribute : DataAttribute
  {
    private static string[] s_dictionaryTypes = new[]
    {
      "IDictionary",
      "IReadOnlyDictionary",
      "Dictionary",
      "SortedDictionary",
      "KeyValuePair"
    };

    private static string[] s_keyTypes = new[]
    {
      "bool",
      "byte",
      "System.DateTime",
      "System.DateTimeOffset",
      "decimal",
      "double",
      "System.Guid",
      "short",
      "int",
      "long",
      "object",
      "sbyte",
      "float",
      "string",
      "ushort",
      "uint",
      "ulong"
    };

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      foreach (string dictionaryType in s_dictionaryTypes)
      {
        foreach (string keyType in s_keyTypes)
        {
          yield return new object?[] { dictionaryType, keyType };
        }
      }
    }
  }

  private class CAEPACTR602WrongSignatureDataAttribute : DataAttribute
  {
    private static string[] s_createAsyncReturnTypes = new[]
    {
      "ValueTask<IMyActor>",
      "object",
      "ValueTask<object>"
    };

    private static string[] s_createAsyncParameters = new[]
    {
      "string id, int state, CancellationToken cancellationToken",
      "int state, CancellationToken cancellationToken",
      "string id, int state",
      "int state",
      "",
      "int id, int state, CancellationToken cancellationToken",
      "string id, int state, string state0",
      "string id, CancellationToken cancellationToken",
      "string id, char state, CancellationToken cancellationToken",
      "string id, int state, string state0, CancellationToken cancellationToken"
    };

    private static string[] s_getReturnTypes = new[]
    {
      "IMyActor",
      "object"
    };

    private static string[] s_getParameters = new[]
    {
      "string id",
      "",
      "int id",
      "string id, int state"
    };

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      for (int i1 = 0; i1 < s_createAsyncReturnTypes.Length; i1++)
      {
        for (int i2 = 0; i2 < s_createAsyncParameters.Length; i2++)
        {
          for (int i3 = 0; i3 < s_getReturnTypes.Length; i3++)
          {
            for (int i4 = 0; i4 < s_getParameters.Length; i4++)
            {
              if (i1 == 0 && i2 == 0 && i3 == 0 && i4 == 0)
                continue;

              yield return new object?[]
              {
                s_createAsyncReturnTypes[i1],
                s_createAsyncParameters[i2],
                s_getReturnTypes[i3],
                s_getParameters[i4]
              };
            }
          }
        }
      }
    }
  }
}
