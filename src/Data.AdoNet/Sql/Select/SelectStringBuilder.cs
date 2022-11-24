using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Sql.Select;

internal delegate void AppendAction<in T>(SelectStringBuilder stringBuilder, T current);
internal delegate void AppendAction<TState, in T>(SelectStringBuilder stringBuilder, in TState state, T current);

internal readonly struct SelectStringBuilder
{
  private readonly StringBuilder _stringBuilder;

  public SelectStringBuilder()
  {
    _stringBuilder = new();
  }

  public void Append(string value)
  {
    _stringBuilder.Append(value);
  }

  public void Append(char value)
  {
    _stringBuilder.Append(value);
  }

  public void Append(int value)
  {
    _stringBuilder.Append(value);
  }

  public void AppendLine()
  {
    _stringBuilder.AppendLine();
  }

  public void AppendLine(string value)
  {
    _stringBuilder.AppendLine(value);
  }

  public override string ToString()
  {
    return _stringBuilder.ToString();
  }

  public void AppendJoin<T>(string separator, IEnumerable<T> values, AppendAction<T> append)
  {
    using IEnumerator<T> en = values.GetEnumerator();

    if (!en.MoveNext())
      return;

    append(this, en.Current);

    while (en.MoveNext())
    {
      _stringBuilder.Append(separator);
      append(this, en.Current);
    }
  }

  public void AppendJoin<TState, T>(string separator, in TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    using IEnumerator<T> en = values.GetEnumerator();

    if (!en.MoveNext())
      return;

    append(this, in state, en.Current);

    while (en.MoveNext())
    {
      _stringBuilder.Append(separator);
      append(this, in state, en.Current);
    }
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
    new AppendJoinCondition(this).Visit(navigation);
  }

  public void AppendTargetUnaliasedColumn(in IndexPair state, IPropertyModel property)
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


  public void AppendChildrenColumns(IReadOnlyList<INavigation> children)
  {
    AppendJoin(", ", children, static (stringBuilder, child) => stringBuilder.AppendColumns(child));
  }

  public void AppendNodeColumns(int index, INavigationNode navigation)
  {
    IndexPair state = new(index, navigation.Index);

    AppendJoin(", ", in state, navigation.Target.Properties, AppendTargetUnaliasedColumn);
    Append(", ");
    AppendJoin(", ", in index, navigation.Children, AppendChildrenColumns);

    static void AppendTargetUnaliasedColumn(SelectStringBuilder stringBuilder, in IndexPair state, IPropertyModel property)
    {
      stringBuilder.AppendTargetUnaliasedColumn(in state, property);
    }

    static void AppendChildrenColumns(SelectStringBuilder stringBuilder, in int index, INavigation child)
    {
      new AppendUnaliasedColumns(stringBuilder).Visit(child, index);
    }
  }

  public void AppendLeafAliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair state = new(index, navigation.Index);

    AppendJoin(", ", in state, navigation.Target.Properties, AppendTargetAliasedColumn);

    static void AppendTargetAliasedColumn(SelectStringBuilder stringBuilder, in IndexPair state, IPropertyModel property)
    {
      stringBuilder.AppendTargetAliasedColumn(in state, property);
    }
  }

  public void AppendLeafUnaliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair state = new(index, navigation.Index);

    AppendJoin(", ", in state, navigation.Target.Properties, AppendTargetUnliasedColumn);

    static void AppendTargetUnliasedColumn(SelectStringBuilder stringBuilder, in IndexPair state, IPropertyModel property)
    {
      stringBuilder.AppendTargetUnaliasedColumn(in state, property);
    }
  }
}
