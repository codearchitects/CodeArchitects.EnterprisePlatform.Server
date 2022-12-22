using System.Diagnostics;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;

internal static class EFCoreEnvironment
{
  public static readonly bool IsMigration = Process.GetCurrentProcess().ProcessName == "dotnet";
}
