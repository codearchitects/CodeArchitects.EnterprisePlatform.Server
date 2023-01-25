using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal readonly struct AppendJoinConditions : INavigationVisitor<VoidResult>
{
  private readonly SqlStringBuilder _stringBuilder;

  public AppendJoinConditions(SqlStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Visit(INavigation navigation)
  {
    navigation.Accept<AppendJoinConditions, VoidResult>(in this);
  }

  public VoidResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation)
  {
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.KeyPairs, AppendCondition);

    return VoidResult.Instance;

    static void AppendCondition(in SqlStringBuilder stringBuilder, ISimpleNavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.FromColumn);
      stringBuilder.Append(" = ");
      stringBuilder.AppendColumn(pair.ToColumn, navigation.Model.Id);
    }
  }

  public VoidResult VisitSimpleNode(ISimpleNavigationNode navigation)
  {
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.KeyPairs, AppendCondition);

    return VoidResult.Instance;

    static void AppendCondition(in SqlStringBuilder stringBuilder, ISimpleNavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.FromColumn);
      stringBuilder.Append(" = ");
      stringBuilder.AppendColumn(pair.ToColumn, navigation.Model.Id, navigation.Model.Id);
    }
  }

  public VoidResult VisitSkipLeaf(ISkipNavigationLeaf navigation)
  {
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeyPairs, AppendCondition);

    return VoidResult.Instance;

    static void AppendCondition(in SqlStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.FromColumn);
      stringBuilder.Append(" = ");
      stringBuilder.AppendColumn(pair.ToColumn, navigation.Model.Id);
    }
  }

  public VoidResult VisitSkipNode(ISkipNavigationNode navigation)
  {
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeyPairs, AppendCondition);

    return VoidResult.Instance;

    static void AppendCondition(in SqlStringBuilder stringBuilder, ISkipNavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.FromColumn);
      stringBuilder.Append(" = ");
      stringBuilder.AppendColumn(pair.ToColumn, navigation.Model.Id);
    }
  }
}
