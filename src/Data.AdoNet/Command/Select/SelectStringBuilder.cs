using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Command.Select;

internal delegate void AppendAction<in T>(SelectStringBuilder stringBuilder, T current);
internal delegate void AppendAction<TState, in T>(SelectStringBuilder stringBuilder, TState state, T current);

internal readonly struct SelectStringBuilder
{
  private readonly StringBuilder _stringBuilder;

  public SelectStringBuilder()
  {
    _stringBuilder = new();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Append(string value)
  {
    _stringBuilder.Append(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Append(char value)
  {
    _stringBuilder.Append(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void Append(int value)
  {
    _stringBuilder.Append(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendLine()
  {
    _stringBuilder.AppendLine();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendLine(string value)
  {
    _stringBuilder.AppendLine(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override string ToString()
  {
    return _stringBuilder.ToString();
  }

  public void AppendJoin<T>(string separator, IEnumerable<T> values, AppendAction<T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return;

    append(this, enumerator.Current);

    while (enumerator.MoveNext())
    {
      _stringBuilder.Append(separator);
      append(this, enumerator.Current);
    }
  }

  public void AppendJoin<TState, T>(string separator, in TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return;

    append(this, state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      _stringBuilder.Append(separator);
      append(this, state, enumerator.Current);
    }
  }


  public void AppendSelectFrom(IEntityModel entity, IReadOnlyCollection<INavigation> navigations)
  {
    Append("SELECT ");
    AppendJoin(", ", entity.Columns, AppendTargetColumn);
    AppendChildrenColumns(navigations);
    AppendLine();
    AppendLine("FROM (");
    Append("SELECT ");
    AppendJoin(", ", entity.Columns, AppendColumn);
    AppendLine();
    Append("FROM [");
    Append(entity.TableName);
    AppendLine("]");
    Append("WHERE ");
    AppendJoin(" AND ", entity.PrimaryKey.Columns, AppendWhereCondition);
    AppendLine();
    Append(") AS t");

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, IColumnModel column)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(column.Name);
      stringBuilder.Append(']');
    }

    static void AppendColumn(SelectStringBuilder stringBuilder, IColumnModel column)
    {
      stringBuilder.Append('[');
      stringBuilder.Append(column.Name);
      stringBuilder.Append(']');
    }

    static void AppendWhereCondition(SelectStringBuilder stringBuilder, IPrimaryKeyColumnModel column)
    {
      stringBuilder.Append('[');
      stringBuilder.Append(column.Name);
      stringBuilder.Append("] = @p");
      stringBuilder.Append(column.PrimaryKeyIndex);
    }
  }

  public void AppendLeftJoin(INavigation child)
  {
    AppendLine();
    Append("LEFT JOIN ");
    AppendTarget(child);
    Append(" AS t");
    Append(child.Model.Id);
    Append(" ON ");
    AppendJoinConditions(child);
  }

  public void AppendColumns(INavigation navigation)
  {
    new AppendColumns(this).Visit(navigation);
  }

  public void AppendTarget(INavigation navigation)
  {
    new AppendTarget(this).Visit(navigation);
  }

  public void AppendJoinConditions(INavigation navigation)
  {
    new AppendJoinConditions(this).Visit(navigation);
  }

  public void AppendTargetUnaliasedColumn(in IndexPair state, IColumnModel column)
  {
    Append('t');
    Append(state.Index);
    Append(".[");
    Append(column.Name);
    Append('_');
    Append(state.NavigationIndex);
    Append(']');
  }

  public void AppendTargetAliasedColumn(in IndexPair state, IColumnModel column)
  {
    Append('t');
    Append(state.Index);
    Append(".[");
    Append(column.Name);
    Append("] AS [");
    Append(column.Name);
    Append("_");
    Append(state.NavigationIndex);
    Append(']');
  }


  public void AppendChildrenColumns(IReadOnlyCollection<INavigation> children)
  {
    if (children.Count == 0)
      return;

    Append(", ");
    AppendJoin(", ", children, static (stringBuilder, child) => stringBuilder.AppendColumns(child));
  }

  public void AppendNodeColumns(int index, INavigationNode navigation)
  {
    IndexPair state = new(index, navigation.Model.Id);

    AppendJoin(", ", state, navigation.Target.Columns, AppendTargetUnaliasedColumn);
    Append(", ");
    AppendJoin(", ", index, navigation.Children, AppendChildrenColumns);

    static void AppendTargetUnaliasedColumn(SelectStringBuilder stringBuilder, IndexPair state, IColumnModel column)
    {
      stringBuilder.AppendTargetUnaliasedColumn(in state, column);
    }

    static void AppendChildrenColumns(SelectStringBuilder stringBuilder, int index, INavigation child)
    {
      new AppendUnaliasedColumns(stringBuilder).Visit(child, index);
    }
  }

  public void AppendLeafAliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair state = new(index, navigation.Model.Id);

    AppendJoin(", ", state, navigation.Target.Columns, AppendTargetAliasedColumn);

    static void AppendTargetAliasedColumn(SelectStringBuilder stringBuilder, IndexPair state, IColumnModel column)
    {
      stringBuilder.AppendTargetAliasedColumn(in state, column);
    }
  }

  public void AppendLeafUnaliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair state = new(index, navigation.Model.Id);

    AppendJoin(", ", state, navigation.Target.Columns, AppendTargetUnliasedColumn);

    static void AppendTargetUnliasedColumn(SelectStringBuilder stringBuilder, IndexPair state, IColumnModel column)
    {
      stringBuilder.AppendTargetUnaliasedColumn(in state, column);
    }
  }
}
