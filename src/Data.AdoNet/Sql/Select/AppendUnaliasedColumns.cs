using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendUnaliasedColumns : INavigationVisitor<VoidResult, int>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendUnaliasedColumns(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

  public VoidResult VisitSkipLeaf(INavigationSkipLeaf navigation, in int state)
  {
    throw new NotImplementedException();
  }

  public VoidResult VisitSkipNode(INavigationSkipNode navigation, in int state)
  {
    throw new NotImplementedException();
  }
}
