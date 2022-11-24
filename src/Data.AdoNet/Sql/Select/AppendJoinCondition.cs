using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendJoinCondition : INavigationVisitor<int>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendJoinCondition(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  public readonly void Visit(INavigation navigation, int index)
  {
    navigation.Accept(in this, in index);
  }

  public readonly void VisitNode(INavigationNode navigation, in int index)
  {
    IndexPair state = new(index, navigation.Index);

    _stringBuilder.AppendJoin(", ", in state, navigation.Model.Keys, AppendCondition);

    static void AppendCondition(SelectStringBuilder stringBuilder, in IndexPair state, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromProperty.ColumnName);
      stringBuilder.Append("] = t");
      stringBuilder.Append(state.Index);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append('_');
      stringBuilder.Append(state.NavigationIndex);
      stringBuilder.Append(']');
    }
  }

  public readonly void VisitLeaf(INavigationLeaf navigation, in int index)
  {
    _stringBuilder.AppendJoin(", ", in index, navigation.Model.Keys, AppendCondition);

    static void AppendCondition(SelectStringBuilder stringBuilder, in int index, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromProperty.ColumnName);
      stringBuilder.Append("] = t");
      stringBuilder.Append(index);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append(']');
    }
  }
}
