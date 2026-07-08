using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal readonly struct AppendNavigationUnaliasedColumns : INavigationVisitor<VoidResult>
{
  private readonly SqlStringBuilder _stringBuilder;
  private readonly int _index;

  public AppendNavigationUnaliasedColumns(SqlStringBuilder stringBuilder, int index)
  {
    _stringBuilder = stringBuilder;
    _index = index;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Visit(INavigation navigation)
  {
    navigation.Accept<AppendNavigationUnaliasedColumns, VoidResult>(in this);
  }

  public VoidResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation)
  {
    _stringBuilder.AppendLeafUnaliasedColumns(_index, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSimpleNode(ISimpleNavigationNode navigation)
  {
    _stringBuilder.AppendNodeColumns(_index, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipLeaf(ISkipNavigationLeaf navigation)
  {
    _stringBuilder.AppendLeafUnaliasedColumns(_index, navigation);

    return VoidResult.Instance;
  }

  public VoidResult VisitSkipNode(ISkipNavigationNode navigation)
  {
    _stringBuilder.AppendNodeColumns(_index, navigation);

    return VoidResult.Instance;
  }
}
