using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal abstract class NavigationNode
{
  private readonly Dictionary<int, INavigation> _children;

  public NavigationNode()
  {
    _children = new();
  }

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

    IAccessibleNavigationModel model;
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
    NavigationNode node = memberExpression.Expression is MemberExpression childMemberExpression
      ? AddNode(childMemberExpression)
      : this;

    IAccessibleNavigationModel model = node.GetNavigationModel(memberExpression.Member);

    node.TryAddLeaf(model);
  }

  public NavigationNode AddNode(MemberExpression memberExpression)
  {
    NavigationNode node = memberExpression.Expression is MemberExpression childMemberExpression
      ? AddNode(childMemberExpression)
      : this;

    IAccessibleNavigationModel model = node.GetNavigationModel(memberExpression.Member);

    return node.GetOrUpdateNode(model);
  }

  private NavigationNode GetOrUpdateNode(IAccessibleNavigationModel model)
  {
    if (!_children.TryGetValue(model.Id, out INavigation? child) || child is not NavigationNode node)
    {
      switch (model)
      {
        case IAccessibleSimpleNavigationModel simpleNavigationModel:
          SimpleNavigationNode simpleNavigationNode = new(simpleNavigationModel);
          _children[model.Id] = simpleNavigationNode;
          return simpleNavigationNode;

        case IAccessibleSkipNavigationModel skipNavigationModel:
          SkipNavigationNode skipNavigationNode = new(skipNavigationModel);
          _children[model.Id] = skipNavigationNode;
          return skipNavigationNode;

        default:
          throw Errors.Unreachable;
      }
    }

    return node;
  }

  private void TryAddLeaf(IAccessibleNavigationModel model)
  {
    if (!_children.ContainsKey(model.Id))
    {
      INavigation leaf = model switch
      {
        IAccessibleSimpleNavigationModel navigationModel   => new SimpleNavigationLeaf(navigationModel),
        IAccessibleSkipNavigationModel skipNavigationModel => new SkipNavigationLeaf(skipNavigationModel),
        _                                                  => throw Errors.Unreachable
      };

      _children.Add(model.Id, leaf);
    }
  }

  private IAccessibleNavigationModel GetNavigationModel(MemberInfo member)
  {
    string navigationName = member switch
    {
      PropertyInfo property => property.Name,
      FieldInfo field       => field.Name,
      _                     => throw new IncludeException("A member of the include expression was not a property or a field.")
    };

    if (!Target.TryGetNavigation(navigationName, out IAccessibleNavigationModel? model))
      throw new IncludeException($"Navigation '{navigationName}' was not found on entity '{Target.Type.Name}'.");

    return model;
  }

  private IAccessibleNavigationModel GetNavigationModel(ReadOnlySpan<char> navigationName)
  {
    if (!Target.TryGetNavigation(navigationName, out IAccessibleNavigationModel? model))
      throw new IncludeException($"Navigation '{navigationName.ToString()}' was not found on entity '{Target.Type.Name}'.");

    return model;
  }
}
