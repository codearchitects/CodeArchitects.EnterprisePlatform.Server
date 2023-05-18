namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Concurrency;

/// <summary>
/// Names for concurrency annotations.
/// </summary>
public static class ConcurrencyAnnotationNames
{
  /// <summary>
  /// The prefix for all annotations.
  /// </summary>
  public const string Prefix = "CA:Concurrency:";

  /// <summary>
  /// The name of the concurrency token annotation.
  /// </summary>
  public const string ConcurrencyToken = Prefix + nameof(ConcurrencyToken);
}
