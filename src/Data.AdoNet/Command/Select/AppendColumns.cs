using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

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

  public VoidResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation)
  {
    _stringBuilder.AppendLeafAliasedColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSimpleNode(ISimpleNavigationNode navigation)
  {
    _stringBuilder.AppendNodeColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipLeaf(ISkipNavigationLeaf navigation)
  {
    _stringBuilder.AppendLeafAliasedColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipNode(ISkipNavigationNode navigation)
  {
    _stringBuilder.AppendNodeColumns(navigation.Model.Id, navigation);

    return VoidResult.Instance;
  }
}