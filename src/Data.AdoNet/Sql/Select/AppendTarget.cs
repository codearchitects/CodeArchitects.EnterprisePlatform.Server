using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Runtime.CompilerServices;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

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

  public VoidResult VisitSimpleLeaf(INavigationSimpleLeaf navigation)
  {
    _stringBuilder.Append("[");
    _stringBuilder.Append(navigation.Target.TableName);
    _stringBuilder.Append(']');

    return VoidResult.Instance;
  }

  public VoidResult VisitSimpleNode(INavigationSimpleNode navigation)
  {
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Properties, AppendTargetColumn);
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
      _stringBuilder.Append(child.Model.Id);
      _stringBuilder.Append(" ON ");
      _stringBuilder.AppendJoinConditions(child);
      _stringBuilder.AppendLine();
    }

    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, INavigationSimpleNode navigation, IPropertyModel property)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append("] AS [");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(']');
    }
  }

  public VoidResult VisitSkipLeaf(INavigationSkipLeaf navigation)
  {
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Properties, AppendTargetColumn);
    _stringBuilder.Append(", ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeys, AppendKey);
    _stringBuilder.AppendLine();
    _stringBuilder.Append("FROM [");
    _stringBuilder.Append(navigation.Model.JoinTableName);
    _stringBuilder.Append("] AS t");
    _stringBuilder.AppendLine();
    _stringBuilder.Append("INNER JOIN [");
    _stringBuilder.Append(navigation.Target.TableName);
    _stringBuilder.Append("] AS t");
    _stringBuilder.Append(navigation.Model.Id);
    _stringBuilder.Append(" ON ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.ToKeys, AppendJoinCondition);
    _stringBuilder.AppendLine();
    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, INavigationSkipLeaf navigation, IPropertyModel property)
    {
      stringBuilder.Append('t');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append(']');
    }

    static void AppendKey(SelectStringBuilder stringBuilder, INavigationSkipLeaf navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append(']');
    }

    static void AppendJoinCondition(SelectStringBuilder stringBuilder, INavigationSkipLeaf navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromProperty.ColumnName);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append(']');
    }
  }

  public VoidResult VisitSkipNode(INavigationSkipNode navigation)
  {
    _stringBuilder.AppendLine("(");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendNodeColumns(navigation.Model.Id, navigation);
    _stringBuilder.Append(", ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.FromKeys, AppendKey);
    _stringBuilder.AppendLine();
    _stringBuilder.Append("FROM [");
    _stringBuilder.Append(navigation.Model.JoinTableName);
    _stringBuilder.AppendLine("] AS t");
    _stringBuilder.AppendLine("INNER JOIN (");
    _stringBuilder.Append("SELECT ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Target.Properties, AppendTargetColumn);
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
      _stringBuilder.Append(child.Model.Id);
      _stringBuilder.Append(" ON ");
      _stringBuilder.AppendJoinConditions(child);
      _stringBuilder.AppendLine();
    }

    _stringBuilder.Append(") AS t");
    _stringBuilder.Append(navigation.Model.Id);
    _stringBuilder.Append(" ON ");
    _stringBuilder.AppendJoin(", ", navigation, navigation.Model.ToKeys, AppendJoinCondition);
    _stringBuilder.AppendLine();
    _stringBuilder.Append(")");

    return VoidResult.Instance;

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, INavigationSkipNode navigation, IPropertyModel property)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append("] AS [");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(']');
    }

    static void AppendKey(SelectStringBuilder stringBuilder, INavigationSkipNode navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append(']');
    }

    static void AppendJoinCondition(SelectStringBuilder stringBuilder, INavigationSkipNode navigation, IKeyPair pair)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(pair.FromProperty.ColumnName);
      stringBuilder.Append("] = t");
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(".[");
      stringBuilder.Append(pair.ToProperty.ColumnName);
      stringBuilder.Append('_');
      stringBuilder.Append(navigation.Model.Id);
      stringBuilder.Append(']');
    }
  }
}
