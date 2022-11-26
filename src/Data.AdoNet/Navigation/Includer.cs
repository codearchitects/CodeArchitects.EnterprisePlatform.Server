using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class Includer<TEntity> : IncluderBase<TEntity>, IIncluder<TEntity>
  where TEntity : class
{
  private readonly NavigationRoot _root;

  public Includer(IEntityModel entity)
  {
    _root = new NavigationRoot(entity);
  }

  protected override IncluderNode Node => _root;

  public INavigationRoot Root => _root;

  public IStringIncluder<TEntity> Include(string navigation)
  {
    try
    {
      _root.AddLeaf(navigation.AsSpan());

      return this;
    }
    catch (IncludeException ex)
    {
      throw WrapIncludeException(ex, nameof(navigation));
    }
  }
}
