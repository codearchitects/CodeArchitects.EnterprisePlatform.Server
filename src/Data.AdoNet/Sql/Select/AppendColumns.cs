using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendColumns : INavigationVisitor
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendColumns(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  public readonly void Visit(INavigation navigation)
  {
    navigation.Accept(in this);
  }

  public readonly void VisitNode(INavigationNode navigation)
  {
    _stringBuilder.AppendNodeColumns(navigation.Index, navigation);
  }

  public readonly void VisitLeaf(INavigationLeaf navigation)
  {
    _stringBuilder.AppendLeafAliasedColumns(navigation.Index, navigation);
  }
}