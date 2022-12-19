using CodeArchitects.Platform.Data.AdoNet.Command.Select;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Runtime.CompilerServices;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal delegate void AppendAction<in T>(in SqlStringBuilder stringBuilder, T current);
internal delegate void AppendAction<TState, in T>(in SqlStringBuilder stringBuilder, TState state, T current);

internal struct SqlStringBuilder
{
  private const string s_tableAlias = "t";

  private readonly StringBuilder _stringBuilder;
  private readonly ISyntaxProvider _syntaxProvider;

  public SqlStringBuilder(ISyntaxProvider syntaxProvider)
    : this(syntaxProvider, 0)
  {
  }

	public SqlStringBuilder(ISyntaxProvider syntaxProvider, int capacity)
	{
    _syntaxProvider = syntaxProvider;
		_stringBuilder = new(capacity);
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
  public void AppendLine(char value)
  {
    _stringBuilder.Append(value);
    _stringBuilder.AppendLine();
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendEscaped(string name)
  {
    _stringBuilder.Append(_syntaxProvider.EscapeLeft);
    _stringBuilder.Append(name);
    _stringBuilder.Append(_syntaxProvider.EscapeRight);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendEscapedWithIndex(string name, int index)
  {
    _stringBuilder.Append(_syntaxProvider.EscapeLeft);
    _stringBuilder.Append(name);
    _stringBuilder.Append('_');
    _stringBuilder.Append(index);
    _stringBuilder.Append(_syntaxProvider.EscapeRight);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendJoinConditions(INavigation navigation)
  {
    new AppendJoinConditions(this).Visit(navigation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendNavigationColumns(INavigation navigation)
  {
    new AppendNavigationColumns(this).Visit(navigation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendNavigationTarget(INavigation navigation)
  {
    new AppendNavigationTarget(this).Visit(navigation);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public void AppendNavigationUnaliasedColumns(INavigation navigation, int index)
  {
    new AppendNavigationUnaliasedColumns(this, index).Visit(navigation);
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

    append(in this, enumerator.Current);

    while (enumerator.MoveNext())
    {
      _stringBuilder.Append(separator);
      append(in this, enumerator.Current);
    }
  }

  public void AppendJoin<TState, T>(string separator, in TState state, IEnumerable<T> values, AppendAction<TState, T> append)
  {
    using IEnumerator<T> enumerator = values.GetEnumerator();

    if (!enumerator.MoveNext())
      return;

    append(in this, state, enumerator.Current);

    while (enumerator.MoveNext())
    {
      _stringBuilder.Append(separator);
      append(in this, state, enumerator.Current);
    }
  }


  public void AppendParameter(IColumnModel column)
  {
    Append(_syntaxProvider.ParameterPrefix);
    Append('p');
    Append(column.Index);
  }

  public void AppendParameters(IEnumerable<IColumnModel> columns)
  {
    AppendJoin(", ", columns, AppendParameter);

    static void AppendParameter(in SqlStringBuilder stringBuilder, IColumnModel column)
    {
      stringBuilder.AppendParameter(column);
    }
  }

  public void AppendColumn(IColumnModel column)
  {
    Append($"{s_tableAlias}.");
    AppendEscaped(column.Name);
  }

  public void AppendColumn(IColumnModel column, int tableIndex)
  {
    Append(s_tableAlias);
    Append(tableIndex);
    Append('.');
    AppendEscaped(column.Name);
  }

  public void AppendColumn(IColumnModel column, int tableIndex, int columnIndex)
  {
    Append(s_tableAlias);
    Append(tableIndex);
    Append('.');
    AppendEscapedWithIndex(column.Name, columnIndex);
  }

  public void AppendColumns(IEnumerable<IColumnModel> columns)
  {
    AppendJoin(", ", columns, AppendColumn);

    static void AppendColumn(in SqlStringBuilder stringBuilder, IColumnModel column)
    {
      stringBuilder.AppendColumn(column);
    }
  }

  public void AppendWhereConditions(IEnumerable<IColumnModel> columns)
  {
    AppendJoin(" AND ", columns, AppendWhereCondition);

    static void AppendWhereCondition(in SqlStringBuilder stringBuilder, IColumnModel column)
    {
      stringBuilder.AppendEscaped(column.Name);
      stringBuilder.Append(" = ");
      stringBuilder.AppendParameter(column);
    }
  }

  public void AppendChildrenColumns(IReadOnlyCollection<INavigation> children)
  {
    if (children.Count == 0)
      return;

    Append(", ");
    AppendJoin(", ", children, AppendColumns);

    static void AppendColumns(in SqlStringBuilder stringBuilder, INavigation navigation)
    {
      stringBuilder.AppendNavigationColumns(navigation);
    }
  }

  public void AppendTableAlias()
  {
    Append(' ');
    if (_syntaxProvider.AppendASKeyword)
    {
      Append("AS ");
    }
    Append(s_tableAlias);
  }

  public void AppendTableAlias(int index)
  {
    AppendTableAlias();
    Append(index);
  }

  public void AppendLeftJoin(INavigation child)
  {
    Append("LEFT JOIN ");
    AppendNavigationTarget(child);
    AppendTableAlias(child.Model.Id);
    Append(" ON ");
    AppendJoinConditions(child);
  }

  public void AppendOutputBefore(IEntityModel entity)
  {
    if (entity.AutoGeneratedColumn is not { } autoGeneratedColumn || !_syntaxProvider.HasOutputBefore)
      return;

    Append(' ');
    Append(_syntaxProvider.GetOutputBefore(entity.TableName, autoGeneratedColumn.Name));
  }

  public void AppendOutputAfter(IEntityModel entity)
  {
    if (entity.AutoGeneratedColumn is not { } autoGeneratedColumn || !_syntaxProvider.HasOutputAfter)
      return;

    Append(' ');
    Append(_syntaxProvider.GetOutputAfter(entity.TableName, autoGeneratedColumn.Name));
  }

  public void AppendNodeColumns(int index, INavigationNode navigation)
  {
    IndexPair indexPair = new(index, navigation.Model.Id);

    AppendJoin(", ", indexPair, navigation.Target.Columns, AppendColumn);
    Append(", ");
    AppendJoin(", ", index, navigation.Children, AppendChildrenColumns);

    static void AppendColumn(in SqlStringBuilder stringBuilder, IndexPair indexPair, IColumnModel column)
    {
      stringBuilder.AppendColumn(column, indexPair.Index, indexPair.NavigationIndex);
    }

    static void AppendChildrenColumns(in SqlStringBuilder stringBuilder, int index, INavigation child)
    {
      stringBuilder.AppendNavigationUnaliasedColumns(child, index);
    }
  }

  public void AppendLeafAliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair indexPair = new(index, navigation.Model.Id);

    AppendJoin(", ", indexPair, navigation.Target.Columns, AppendTargetAliasedColumn);

    static void AppendTargetAliasedColumn(in SqlStringBuilder stringBuilder, IndexPair indexPair, IColumnModel column)
    {
      stringBuilder.AppendColumn(column, indexPair.Index);
      stringBuilder.Append(" AS ");
      stringBuilder.AppendEscapedWithIndex(column.Name, indexPair.NavigationIndex);
    }
  }

  public void AppendLeafUnaliasedColumns(int index, INavigationLeaf navigation)
  {
    IndexPair indexPair = new(index, navigation.Model.Id);

    AppendJoin(", ", indexPair, navigation.Target.Columns, AppendColumn);

    static void AppendColumn(in SqlStringBuilder stringBuilder, IndexPair indexPair, IColumnModel column)
    {
      stringBuilder.AppendColumn(column, indexPair.Index, indexPair.NavigationIndex);
    }
  }
}
