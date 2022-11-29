namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class ThenIncluder<TEntity> : IncluderBase<TEntity>
  where TEntity : class
{
  private readonly IncluderNode _node;

  public ThenIncluder(IncluderNode node)
  {
    _node = node;
  }

  protected override IncluderNode Node => _node;
}
