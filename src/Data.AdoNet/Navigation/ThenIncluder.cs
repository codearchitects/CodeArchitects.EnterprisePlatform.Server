namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class ThenIncluder<TEntity> : IncluderBase<TEntity>
  where TEntity : class
{
  private readonly NavigationNode _node;

  public ThenIncluder(NavigationNode node)
  {
    _node = node;
  }

  protected override IncluderNode Node => _node;
}
