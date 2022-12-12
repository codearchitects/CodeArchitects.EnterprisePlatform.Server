namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class ThenIncluder<TEntity> : Includer<TEntity>
  where TEntity : class
{
  public ThenIncluder(IncluderNode node)
  {
    Node = node;
  }

  protected override IncluderNode Node { get; }
}
