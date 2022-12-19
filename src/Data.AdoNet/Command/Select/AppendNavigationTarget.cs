using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Visitors;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal readonly struct AppendNavigationTarget : INavigationVisitor<VoidResult>
{
  private readonly SqlStringBuilder _stringBuilder;

  public AppendNavigationTarget(SqlStringBuilder stringBuilder)
  {
    _stringBuilder = stringBuilder;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Visit(INavigation navigation)
  {
    navigation.Accept<AppendNavigationTarget, VoidResult>(in this);
  }

  public VoidResult VisitSimpleLeaf(ISimpleNavigationLeaf navigation)
  {
    _stringBuilder.AppendEscaped(navigation.Target.TableName);

    return VoidResult.Instance;
  }

  public VoidResult VisitSimpleNode(ISimpleNavigationNode navigation)
  {
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Columns, AppendTargetColumn);
    _stringBuilder.AppendChildrenColumns(navigation.Children);
    _stringBuilder.AppendLine();
    _stringBuilder.Append("FROM ");
    _stringBuilder.AppendEscaped(navigation.Target.TableName);
    _stringBuilder.AppendLine(" AS t");

    foreach (INavigation child in navigation.Children)
    {
      _stringBuilder.Append("LEFT JOIN ");
      _stringBuilder.AppendNavigationTarget(child);
      _stringBuilder.Append($" AS {SqlStringBuilder.TableAlias}");
      _stringBuilder.Append(child.Model.Id);
      _stringBuilder.Append(" ON ");
      _stringBuilder.AppendJoinConditions(child);
      _stringBuilder.AppendLine();
    }

    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(in SqlStringBuilder stringBuilder, ISimpleNavigationNode navigation, IColumnModel column)
    {
      stringBuilder.AppendColumn(column);
      stringBuilder.Append(" AS ");
      stringBuilder.AppendEscapedWithIndex(column.Name, navigation.Model.Id);
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
    _stringBuilder.Append("FROM ");
    _stringBuilder.AppendEscaped(navigation.Model.JoinEntity.TableName);
    _stringBuilder.Append($" AS {SqlStringBuilder.TableAlias}");
    _stringBuilder.AppendLine();
    _stringBuilder.Append("INNER JOIN ");
    _stringBuilder.AppendEscaped(navigation.Target.TableName);
    _stringBuilder.Append($" AS {SqlStringBuilder.TableAlias}");
    _stringBuilder.Append(navigation.Model.Id);
    _stringBuilder.Append(" ON ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.ToKeyPairs, AppendJoinCondition);
    _stringBuilder.AppendLine();
    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(in SqlStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IColumnModel column)
    {
      stringBuilder.AppendColumn(column, navigation.Model.Id);
    }

    static void AppendKey(in SqlStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.ToColumn);
    }

    static void AppendJoinCondition(in SqlStringBuilder stringBuilder, ISkipNavigationLeaf navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.FromColumn);
      stringBuilder.Append(" = ");
      stringBuilder.AppendColumn(pair.ToColumn, navigation.Model.Id);
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
    _stringBuilder.Append("FROM ");
    _stringBuilder.AppendEscaped(navigation.Model.JoinEntity.TableName);
    _stringBuilder.AppendLine($" AS {SqlStringBuilder.TableAlias}");
    _stringBuilder.AppendLine("INNER JOIN (");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Columns, AppendTargetColumn);
    _stringBuilder.AppendChildrenColumns(navigation.Children);
    _stringBuilder.AppendLine();
    _stringBuilder.Append("FROM ");
    _stringBuilder.AppendEscaped(navigation.Target.TableName);
    _stringBuilder.AppendLine($" AS {SqlStringBuilder.TableAlias}");

    foreach (INavigation child in navigation.Children)
    {
      _stringBuilder.Append("LEFT JOIN ");
      _stringBuilder.AppendNavigationTarget(child);
      _stringBuilder.Append($" AS {SqlStringBuilder.TableAlias}");
      _stringBuilder.Append(child.Model.Id);
      _stringBuilder.Append(" ON ");
      _stringBuilder.AppendJoinConditions(child);
      _stringBuilder.AppendLine();
    }

    _stringBuilder.Append($") AS {SqlStringBuilder.TableAlias}");
    _stringBuilder.Append(navigation.Model.Id);
    _stringBuilder.Append(" ON ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.ToKeyPairs, AppendJoinCondition);
    _stringBuilder.AppendLine();
    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(in SqlStringBuilder stringBuilder, ISkipNavigationNode navigation, IColumnModel column)
    {
      stringBuilder.AppendColumn(column);
      stringBuilder.Append(" AS ");
      stringBuilder.AppendEscapedWithIndex(column.Name, navigation.Model.Id);
    }

    static void AppendKey(in SqlStringBuilder stringBuilder, ISkipNavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.ToColumn);
    }

    static void AppendJoinCondition(in SqlStringBuilder stringBuilder, ISkipNavigationNode navigation, IKeyPair pair)
    {
      stringBuilder.AppendColumn(pair.FromColumn);
      stringBuilder.Append(" = ");
      stringBuilder.AppendColumn(pair.ToColumn, navigation.Model.Id, navigation.Model.Id);
    }
  }
}
