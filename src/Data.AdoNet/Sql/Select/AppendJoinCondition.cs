using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendJoinCondition : INavigationVisitor
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendJoinCondition(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  public readonly void Visit(INavigation navigation)
  {
    navigation.Accept(in this);
  }

  public readonly void VisitNode(INavigationNode navigation)
  {
    _stringBuilder.AppendJoin(", ", in navigation, navigation.Model.Keys, AppendCondition);

    static void AppendCondition(SelectStringBuilder stringBuilder, in INavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromProperty.ColumnName);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Index);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Index);
      stringBuilder.Append(']');
    }
  }

  public readonly void VisitLeaf(INavigationLeaf navigation)
  {
    _stringBuilder.AppendJoin(", ", in navigation, navigation.Model.Keys, AppendCondition);

    static void AppendCondition(SelectStringBuilder stringBuilder, in INavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromProperty.ColumnName);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Index);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append(']');
    }
  }
}
