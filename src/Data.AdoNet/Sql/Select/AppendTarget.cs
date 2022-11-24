using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal readonly struct AppendTarget : INavigationVisitor
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendTarget(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  public readonly void Visit(INavigation navigation)
  {
    navigation.Accept(in this);
  }

  public readonly void VisitNode(INavigationNode navigation)
  {
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", in navigation, navigation.Target.Properties, AppendTargetColumn);
    _stringBuilder.Append(", ");
    _stringBuilder.AppendChildrenColumns(navigation.Children);
    _stringBuilder.AppendLine();
    _stringBuilder.Append("FROM [");
    _stringBuilder.Append(navigation.Target.TableName);
    _stringBuilder.AppendLine("] AS t");

    foreach (INavigation child in navigation.Children)
    {
      _stringBuilder.Append("LEFT JOIN ");
      _stringBuilder.AppendTarget(child);
      _stringBuilder.Append(" AS t");
      _stringBuilder.Append(child.Index);
      _stringBuilder.Append(" ON ");
      _stringBuilder.AppendJoinConditions(child);
    }

    _stringBuilder.AppendLine();
    _stringBuilder.Append(")");

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, in INavigationNode navigation, IPropertyModel property)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append("] AS [");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Index);
      stringBuilder.Append(']');
    }
  }

  public readonly void VisitLeaf(INavigationLeaf navigation)
  {
    _stringBuilder.Append("[");
    _stringBuilder.Append(navigation.Target.TableName);
    _stringBuilder.Append(']');
  }
}
