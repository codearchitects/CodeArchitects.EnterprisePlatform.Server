using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal abstract class IncluderNode
{
  private readonly Dictionary<int, INavigation> _children;

  public IncluderNode()
  {
    _children = new();
  }

  protected bool IsFrozen { get; private set; }

  public abstract IEntityModel Target { get; }

  public IReadOnlyCollection<INavigation> Children => _children.Values;

  public void AddLeaf(ReadOnlySpan<char> path)
  {
    int index = 0;
    int length = path.Length;

    if (length == 0)
      throw new IncludeException("A part of the include path was empty.");

    while (index < length && path[index] is not '.')
    {
      index++;
    }

    INavigationModel model;
    if (index == length)
    {
      model = GetNavigationModel(path);
      TryAddLeaf(model);

      return;
    }

    model = GetNavigationModel(path[..index]);
    NavigationNode node = GetOrUpdateNode(model);

    node.AddLeaf(path[(index + 1)..]);
  }

  public void AddLeaf(MemberExpression memberExpression)
  {
    IncluderNode node = memberExpression.Expression is MemberExpression childMemberExpression
      ? AddNode(childMemberExpression)
      : this;

    INavigationModel model = node.GetNavigationModel(memberExpression.Member);

    node.TryAddLeaf(model);
  }

  public NavigationNode AddNode(MemberExpression memberExpression)
  {
    IncluderNode node = memberExpression.Expression is MemberExpression childMemberExpression
      ? AddNode(childMemberExpression)
      : this;

    INavigationModel model = node.GetNavigationModel(memberExpression.Member);

    return node.GetOrUpdateNode(model);
  }

  private NavigationNode GetOrUpdateNode(INavigationModel model)
  {
    if (!_children.TryGetValue(model.Id, out INavigation? child) || child is not NavigationNode childNode)
    {
      childNode = new(model);
      _children[model.Id] = childNode;
    }

    return childNode;
  }

  private void TryAddLeaf(INavigationModel model)
  {
    if (!_children.ContainsKey(model.Id))
    {
      _children.Add(model.Id, new NavigationLeaf(model));
    }
  }

  private INavigationModel GetNavigationModel(MemberInfo member)
  {
    string navigationName = member switch
    {
      PropertyInfo property => property.Name,
      FieldInfo field       => field.Name,
      _                     => throw new IncludeException("A member of the include expression was not a property or a field.")
    };

    if (!Target.TryGetNavigation(navigationName, out INavigationModel? model))
      throw new IncludeException($"Navigation '{navigationName}' was not found on entity '{Target.Name}'.");

    return model;
  }

  private INavigationModel GetNavigationModel(ReadOnlySpan<char> navigationName)
  {
    if (!Target.TryGetNavigation(navigationName, out INavigationModel? model))
      throw new IncludeException($"Navigation '{navigationName.ToString()}' was not found on entity '{Target.Name}'.");

    return model;
  }
}
