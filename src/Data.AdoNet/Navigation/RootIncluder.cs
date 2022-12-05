using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal class RootIncluder<TEntity, TKey> : Includer<TEntity>, IIncluder<TEntity>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private readonly NavigationRoot<TEntity, TKey> _root;

  public RootIncluder(IEntityModel<TEntity, TKey> entity)
  {
    _root = new NavigationRoot<TEntity, TKey>(entity);
  }

  protected override IncluderNode Node => _root;

  public NavigationSpec<TEntity, TKey> Spec => NavigationSpec.FromNavigation(_root);

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
