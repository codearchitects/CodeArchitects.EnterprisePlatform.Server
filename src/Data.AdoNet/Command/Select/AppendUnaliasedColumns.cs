using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal readonly struct AppendUnaliasedColumns : INavigationVisitor<VoidResult, int>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendUnaliasedColumns(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Visit(INavigation navigation, int index)
  {
    navigation.Accept<AppendUnaliasedColumns, VoidResult, int>(in this, in index);
  }

  public VoidResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation, in int index)
  {
    _stringBuilder.AppendLeafUnaliasedColumns(index, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSimpleNode(ISimpleNavigationNode navigation, in int index)
  {
    _stringBuilder.AppendNodeColumns(index, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipLeaf(ISkipNavigationLeaf navigation, in int index)
  {
    _stringBuilder.AppendLeafUnaliasedColumns(index, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipNode(ISkipNavigationNode navigation, in int index)
  {
    _stringBuilder.AppendNodeColumns(index, navigation);

    return VoidResult.Instance;
  }
}
