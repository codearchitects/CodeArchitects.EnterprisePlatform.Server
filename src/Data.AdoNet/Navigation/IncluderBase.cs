using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal abstract class IncluderBase
{
  private readonly Dictionary<int, INavigation> _children;

  public IncluderBase()
  {
    _children = new();
  }

  protected abstract IEntityModel Target { get; }

  protected IReadOnlyCollection<INavigation> Children => _children.Values;

  protected bool TryInclude(ReadOnlySpan<char> path)
  {
    int index = 0;
    int length = path.Length;
    INavigationModel? model;

    while (index < length && path[index] is not '.')
    {
      index++;
    }

    if (index == length)
    {
      if (!Target.TryGetNavigation(path, out model))
        return false;

      if (!_children.ContainsKey(model.Id))
      {
        _children.Add(model.Id, new IncluderLeaf(model));
      }

      return true;
    }

    if (index == length - 1)
      return false;

    ReadOnlySpan<char> navigationName = path[..index];
    if (!Target.TryGetNavigation(navigationName, out model))
      return false;

    if (!_children.TryGetValue(model.Id, out INavigation? child) || child is not IncluderNode includer)
    {
      includer = new(model);
      _children[model.Id] = includer;
    }

    return includer.TryInclude(path[(index + 1)..]);
  }
}
