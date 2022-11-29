using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendColumns : INavigationVisitor<VoidResult>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendColumns(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly void Visit(INavigation navigation)
  {
    navigation.Accept<AppendColumns, VoidResult>(in this);
  }

  public readonly VoidResult VisitLeaf(INavigationLeaf navigation)
  {
    _stringBuilder.AppendLeafAliasedColumns(navigation.Id, navigation);
    return VoidResult.Instance;
  }

  public readonly VoidResult VisitNode(INavigationNode navigation)
  {
    _stringBuilder.AppendNodeColumns(navigation.Id, navigation);
    return VoidResult.Instance;
  }

  public VoidResult VisitSkipLeaf(INavigationSkipLeaf navigation)
  {
    throw new NotImplementedException();
  }

  public VoidResult VisitSkipNode(INavigationSkipNode navigation)
  {
    throw new NotImplementedException();
  }
}