namespace CodeArchitects.Platform.Data.Features.SoftDelete;

public interface ISoftDeleteContext
{
  bool ShouldFilter { get; }
}
