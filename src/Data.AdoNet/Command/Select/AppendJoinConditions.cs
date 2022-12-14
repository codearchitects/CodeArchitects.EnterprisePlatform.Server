using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal readonly struct AppendJoinConditions : INavigationVisitor<VoidResult>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendJoinConditions(SelectStringBuilder stringBuilder)
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

    static void AppendCondition(SelectStringBuilder stringBuilder, ISimpleNavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromColumn.Name);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToColumn.Name);
      stringBuilder.Append(']');
    }
  }

  public VoidResult VisitSimpleNode(ISimpleNavigationNode navigation)
  {
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.KeyPairs, AppendCondition);

    return VoidResult.Instance;

    static void AppendCondition(SelectStringBuilder stringBuilder, ISimpleNavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromColumn.Name);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToColumn.Name);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(']');
    }
  }

  public VoidResult VisitSkipLeaf(ISkipNavigationLeaf navigation)
  {
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeyPairs, AppendCondition);

    return VoidResult.Instance;

    static void AppendCondition(SelectStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromColumn.Name);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToColumn.Name);
      stringBuilder.Append(']');
    }
  }

  public VoidResult VisitSkipNode(ISkipNavigationNode navigation)
  {
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeyPairs, AppendCondition);

    return VoidResult.Instance;

    static void AppendCondition(SelectStringBuilder stringBuilder, ISkipNavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromColumn.Name);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToColumn.Name);
      stringBuilder.Append(']');
    }
  }
}
