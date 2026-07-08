namespace CodeArchitects.Platform.Data.Features.SoftDelete;

/// <summary>
/// Provides information about soft delete filtering.
/// </summary>
public interface ISoftDeleteContext
{
  /// <summary>
  /// Indicates whether the data should be filtered to exclude soft deleted items.
  /// </summary>
  bool ShouldFilter { get; }
}
