using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendUnaliasedColumns : INavigationVisitor<int>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendUnaliasedColumns(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  public readonly void Visit(INavigation navigation, int index)
  {
    navigation.Accept(in this, in index);
  }

  public readonly void VisitNode(INavigationNode navigation, in int index)
  {
    _stringBuilder.AppendNodeColumns(in index, navigation);
  }

  public readonly void VisitLeaf(INavigationLeaf navigation, in int index)
  {
    _stringBuilder.AppendLeafUnaliasedColumns(in index, navigation);
  }
}
