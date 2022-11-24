using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendColumns : INavigationVisitor<VoidResult>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendColumns(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  public readonly void Visit(INavigation navigation)
  {
    navigation.Accept<AppendColumns, VoidResult>(in this);
  }

  public readonly VoidResult VisitLeaf(INavigationLeaf navigation)
  {
    _stringBuilder.AppendLeafAliasedColumns(navigation.Index, navigation);
    return VoidResult.Instance;
  }

  public readonly VoidResult VisitNode(INavigationNode navigation)
  {
    _stringBuilder.AppendNodeColumns(navigation.Index, navigation);
    return VoidResult.Instance;
  }
}