using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

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
  public readonly void Append(string value)
  {
    _stringBuilder.Append(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly void Append(char value)
  {
    _stringBuilder.Append(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly void Append(int value)
  {
    _stringBuilder.Append(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly void AppendLine()
  {
    _stringBuilder.AppendLine();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly void AppendLine(string value)
  {
    _stringBuilder.AppendLine(value);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public override string ToString()
  {
    return _stringBuilder.ToString();
  }

  public readonly void AppendJoin<T>(string separator, IEnumerable<T> values, AppendAction<T> append)
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

  public readonly void AppendJoin<TState, T>(string separator, in TState state, IEnumerable<T> values, AppendAction<TState, T> append)
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
    AppendJoin(", ", entity.Properties, AppendTargetColumn);
    Append(", ");
    AppendChildrenColumns(navigations);
    AppendLine();
    AppendLine("FROM (");
    Append("SELECT ");
    AppendJoin(", ", entity.Properties, AppendColumn);
    AppendLine();
    Append("FROM [");
    Append(entity.TableName);
    AppendLine("]");
    Append("WHERE ");
    AppendJoin(" AND ", entity.PrimaryKey.Properties, AppendWhereCondition);
    AppendLine();
    Append(") AS t");

    static void AppendTargetColumn(SelectStringBuilder stringBuilder, IPropertyModel property)
    {
      stringBuilder.Append("t.[");
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append(']');
    }

    static void AppendColumn(SelectStringBuilder stringBuilder, IPropertyModel property)
    {
      stringBuilder.Append('[');
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append(']');
    }

    static void AppendWhereCondition(SelectStringBuilder stringBuilder, IPrimaryKeyPropertyModel property)
    {
      stringBuilder.Append('[');
      stringBuilder.Append(property.ColumnName);
      stringBuilder.Append("] = @p");
      stringBuilder.Append(property.Index);
    }
  }

  public readonly void AppendLeftJoin(INavigation child)
  {
    AppendLine();
    Append("LEFT JOIN ");
    AppendTarget(child);
    Append(" AS t");
    Append(child.Id);
    Append(" ON ");
    AppendJoinConditions(child);
  }

  public readonly void AppendColumns(INavigation navigation)
  {
    new AppendColumns(this).Visit(navigation);
  }

  public readonly void AppendTarget(INavigation navigation)
  {
    new AppendTarget(this).Visit(navigation);
  }

  public readonly void AppendJoinConditions(INavigation navigation)
  {
    new AppendJoinConditions(this).Visit(navigation);
  }

  public readonly void AppendTargetUnaliasedColumn(in IndexPair state, IPropertyModel property)
  {
    Append('t');
    Append(state.Index);
    Append(".[");
    Append(property.ColumnName);
    Append('_');
    Append(state.NavigationIndex);
    Append(']');
  }

  public void AppendTargetAliasedColumn(in IndexPair state, IPropertyModel property)
  {
    Append('t');
    Append(state.Index);
    Append(".[");
    Append(property.ColumnName);
    Append("] AS [");
    Append(property.ColumnName);
    Append("_");
    Append(state.NavigationIndex);
    Append(']');
  }


  public readonly void AppendChildrenColumns(IReadOnlyCollection<INavigation> children)
  {
    AppendJoin(", ", children, static (stringBuilder, child) => stringBuilder.AppendColumns(child));
  }

  public readonly void AppendNodeColumns(int index, INavigationNode navigation)
  {
    IndexPair state = new(index, navigation.Id);

    AppendJoin(", ", state, navigation.Target.Properties, AppendTargetUnaliasedColumn);
    Append(", ");
    AppendJoin(", ", index, navigation.Children, AppendChildrenColumns);

    static void AppendTargetUnaliasedColumn(SelectStringBuilder stringBuilder, IndexPair state, IPropertyModel property)
    {
      stringBuilder.AppendTargetUnaliasedColumn(in state, property);
    }

    static void AppendChildrenColumns(SelectStringBuilder stringBuilder, int index, INavigation child)
    {
      new AppendUnaliasedColumns(stringBuilder).Visit(child, index);
    }
  }

  public readonly void AppendLeafAliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair state = new(index, navigation.Id);

    AppendJoin(", ", state, navigation.Target.Properties, AppendTargetAliasedColumn);

    static void AppendTargetAliasedColumn(SelectStringBuilder stringBuilder, IndexPair state, IPropertyModel property)
    {
      stringBuilder.AppendTargetAliasedColumn(in state, property);
    }
  }

  public readonly void AppendLeafUnaliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair state = new(index, navigation.Id);

    AppendJoin(", ", state, navigation.Target.Properties, AppendTargetUnliasedColumn);

    static void AppendTargetUnliasedColumn(SelectStringBuilder stringBuilder, IndexPair state, IPropertyModel property)
    {
      stringBuilder.AppendTargetUnaliasedColumn(in state, property);
    }
  }
}
