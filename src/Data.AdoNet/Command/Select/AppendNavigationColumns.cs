using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal readonly struct AppendNavigationColumns : INavigationVisitor<VoidResult>
{
  private readonly SqlStringBuilder _stringBuilder;

  public AppendNavigationColumns(SqlStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Visit(INavigation navigation)
  {
    navigation.Accept<AppendNavigationColumns, VoidResult>(in this);
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