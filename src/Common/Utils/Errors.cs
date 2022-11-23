using System.Diagnostics;

namespace CodeArchitects.Platform.Common.Utils;

internal static class Errors
{
  public static Exception Unreacheable => throw new UnreachableException();
}
