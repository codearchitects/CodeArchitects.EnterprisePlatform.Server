namespace CodeArchitects.Platform.Data.Features.SoftDelete;

internal class DefaultSoftDeleteContext : ISoftDeleteContext
{
  public bool ShouldFilter => true;
}
