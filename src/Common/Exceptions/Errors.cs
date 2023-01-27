using System.Diagnostics;

namespace CodeArchitects.Platform.Common.Exceptions;

internal static class Errors
{
  public static Exception Unreachable => throw new UnreachableException();
}
