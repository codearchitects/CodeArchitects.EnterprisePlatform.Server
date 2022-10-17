namespace CodeArchitects.Platform.Common.Utils;

internal static class Errors
{
  public static Exception Unreacheable => throw new InvalidOperationException("This was supposed to be unreacheable.");
}
