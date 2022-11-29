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
  public void Visit(INavigation navigation)
  {
    navigation.Accept<AppendColumns, VoidResult>(in this);
  }

  public VoidResult VisitSimpleLeaf(INavigationSimpleLeaf navigation)
  {
    _stringBuilder.AppendLeafAliasedColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSimpleNode(INavigationSimpleNode navigation)
  {
    _stringBuilder.AppendNodeColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipLeaf(INavigationSkipLeaf navigation)
  {
    _stringBuilder.AppendLeafAliasedColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipNode(INavigationSkipNode navigation)
  {
    _stringBuilder.AppendNodeColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }
}