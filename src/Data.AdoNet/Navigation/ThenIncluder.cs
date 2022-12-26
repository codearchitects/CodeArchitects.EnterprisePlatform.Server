namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class ThenIncluder<TEntity> : Includer<TEntity>
  where TEntity : class
{
  public ThenIncluder(NavigationNode node)
  {
    Node = node;
  }

  protected override NavigationNode Node { get; }
}
