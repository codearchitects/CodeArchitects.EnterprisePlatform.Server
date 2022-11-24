using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendUnaliasedColumns : INavigationVisitor<VoidResult, int>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendUnaliasedColumns(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  public readonly void Visit(INavigation navigation, int index)
  {
    navigation.Accept<AppendUnaliasedColumns, VoidResult, int>(in this, in index);
  }

  public readonly VoidResult VisitLeaf(INavigationLeaf navigation, in int index)
  {
    _stringBuilder.AppendLeafUnaliasedColumns(index, navigation);

    return VoidResult.Instance;
  }

  public readonly VoidResult VisitNode(INavigationNode navigation, in int index)
  {
    _stringBuilder.AppendNodeColumns(index, navigation);

    return VoidResult.Instance;
  }
}
