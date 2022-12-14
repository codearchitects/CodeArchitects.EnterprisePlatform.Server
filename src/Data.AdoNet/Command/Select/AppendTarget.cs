using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal readonly struct AppendTarget : INavigationVisitor<VoidResult>
{
  private readonly SelectStringBuilder _stringBuilder;

  public AppendTarget(SelectStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Visit(INavigation navigation)
  {
    navigation.Accept<AppendTarget, VoidResult>(in this);
  }

  public VoidResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation)
  {
    _stringBuilder.Append("[");
    _stringBuilder.Append(navigation.Target.TableName);
    _stringBuilder.Append(']');

    return VoidResult.Instance;
  }

  public VoidResult VisitSimpleNode(ISimpleNavigationNode navigation)
  {
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Columns, AppendTargetColumn);
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
      _stringBuilder.Append(child.Model.Id);
      _stringBuilder.Append(" ON ");
      _stringBuilder.AppendJoinConditions(child);
      _stringBuilder.AppendLine();
    }

    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, ISimpleNavigationNode navigation, IColumnModel column)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(column.Name);
      stringBuilder.Append("] AS [");
      stringBuilder.Append(column.Name);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(']');
    }
  }

  public VoidResult VisitSkipLeaf(ISkipNavigationLeaf navigation)
  {
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Columns, AppendTargetColumn);
    _stringBuilder.Append(", ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeyPairs, AppendKey);
    _stringBuilder.AppendLine();
    _stringBuilder.Append("FROM [");
    _stringBuilder.Append(navigation.Model.JoinEntity.TableName);
    _stringBuilder.Append("] AS t");
    _stringBuilder.AppendLine();
    _stringBuilder.Append("INNER JOIN [");
    _stringBuilder.Append(navigation.Target.TableName);
    _stringBuilder.Append("] AS t");
    _stringBuilder.Append(navigation.Model.Id);
    _stringBuilder.Append(" ON ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.ToKeyPairs, AppendJoinCondition);
    _stringBuilder.AppendLine();
    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IColumnModel column)
    {
      stringBuilder.Append('t');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(column.Name);
      stringBuilder.Append(']');
    }

    static void AppendKey(SelectStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.ToColumn.Name);
      stringBuilder.Append(']');
    }

    static void AppendJoinCondition(SelectStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IKeyPair pair)
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
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendNodeColumns(navigation.Model.Id, navigation);
    _stringBuilder.Append(", ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeyPairs, AppendKey);
    _stringBuilder.AppendLine();
    _stringBuilder.Append("FROM [");
    _stringBuilder.Append(navigation.Model.JoinEntity.TableName);
    _stringBuilder.AppendLine("] AS t");
    _stringBuilder.AppendLine("INNER JOIN (");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Columns, AppendTargetColumn);
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
      _stringBuilder.Append(child.Model.Id);
      _stringBuilder.Append(" ON ");
      _stringBuilder.AppendJoinConditions(child);
      _stringBuilder.AppendLine();
    }

    _stringBuilder.Append(") AS t");
    _stringBuilder.Append(navigation.Model.Id);
    _stringBuilder.Append(" ON ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.ToKeyPairs, AppendJoinCondition);
    _stringBuilder.AppendLine();
    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, ISkipNavigationNode navigation, IColumnModel column)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(column.Name);
      stringBuilder.Append("] AS [");
      stringBuilder.Append(column.Name);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(']');
    }

    static void AppendKey(SelectStringBuilder stringBuilder, ISkipNavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.ToColumn.Name);
      stringBuilder.Append(']');
    }

    static void AppendJoinCondition(SelectStringBuilder stringBuilder, ISkipNavigationNode navigation, IKeyPair pair)
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
}
