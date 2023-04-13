namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;

internal static class EFCoreEnvironment
{
  public static bool IsDesignTime =>
#if NET6_0_OR_GREATER
    Microsoft.EntityFrameworkCore.EF.IsDesignTime;
#else
    Environment.GetEnvironmentVariable("EFCORE_ISRUNTIME") != "true";
#endif
}
