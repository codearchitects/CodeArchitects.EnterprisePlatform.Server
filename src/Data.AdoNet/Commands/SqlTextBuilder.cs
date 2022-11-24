using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

internal class SqlTextBuilder : ISqlTextBuilder // TODO: Support multiple database providers
{
  private readonly ISqlTextCache _cache;

  public SqlTextBuilder(ISqlTextCache cache)
  {
    _cache = cache;
  }

  public string BuildSelectText(INavigationRoot root)
  {
    if (_cache.TryGetSelectText(root.CacheKey, out string? text))
      return text;

    IEntityModel entity = root.Target;

    StringBuilder stringBuilder = new();

    stringBuilder
      .Append("SELECT ")
      .AppendJoin(", ", entity.Properties, static (stringBuilder, property) => stringBuilder
        .Append("t.[")
        .Append(property.ColumnName)
        .Append(']'))
      .Append(", ")
      .AppendJoin(", ", root.Children, AppendChildrenColumns)
      .AppendLine()
      .AppendLine("FROM (")
      .Append("SELECT ")
      .AppendJoin(", ", entity.Properties, static (stringBuilder, property) => stringBuilder
        .Append('[')
        .Append(property.ColumnName)
        .Append(']'))
      .AppendLine()
      .Append("FROM [")
      .Append(entity.TableName)
      .AppendLine("]")
      .Append("WHERE ")
      .AppendJoin(", ", entity.PrimaryKey.Properties, static (stringBuilder, property) => stringBuilder
        .Append('[')
        .Append(property.ColumnName)
        .Append("] = @p")
        .Append(property.Index))
      .AppendLine()
      .Append(") AS t");

    foreach (INavigation child in root.Children)
    {
      stringBuilder
        .AppendLine()
        .Append("LEFT JOIN ");
      new AppendTarget(stringBuilder).Visit(child, child.Index);
      stringBuilder
        .Append(" AS t")
        .Append(child.Index)
        .Append(" ON ");
      new AppendJoinCondition(stringBuilder).Visit(child, child.Index);
    }

    return stringBuilder.ToString();
  }

  private static void AppendChildrenColumns(StringBuilder stringBuilder, INavigation navigation)
  {
    new AppendColumns(stringBuilder).Visit(navigation, navigation.Index);
  }

  private static void AppendTargetUnaliasedColumn(StringBuilder stringBuilder, in IndexPair state, IPropertyModel property)
  {
    stringBuilder
      .Append('t')
      .Append(state.Index)
      .Append(".[")
      .Append(property.ColumnName)
      .Append('_')
      .Append(state.NavigationIndex)
      .Append(']');
  }

  private static void AppendTargetAliasedColumn(StringBuilder stringBuilder, in IndexPair state, IPropertyModel property)
  {
    stringBuilder
      .Append('t')
      .Append(state.Index)
      .Append(".[")
      .Append(property.ColumnName)
      .Append("] AS [")
      .Append(property.ColumnName)
      .Append("_")
      .Append(state.NavigationIndex)
      .Append(']');
  }

  private static void AppendNodeColumns(StringBuilder stringBuilder, in int index, INavigationNode navigation)
  {
    IndexPair state = new(index, navigation.Index);

    stringBuilder
      .AppendJoin(", ", in state, navigation.Target.Properties, AppendTargetUnaliasedColumn)
      .Append(", ")
      .AppendJoin(", ", in index, navigation.Children, AppendChildrenColumns);


    static void AppendChildrenColumns(StringBuilder stringBuilder, in int index, INavigation child)
    {
      new AppendUnaliasedColumns(stringBuilder).Visit(child, index);
    }
  }

  private static void AppendLeafColumns(StringBuilder stringBuilder, in int index, INavigationLeaf navigation, AppendAction<IndexPair, IPropertyModel> appendTargetColumn)
  {
    IndexPair state = new(index, navigation.Index);

    stringBuilder.AppendJoin(", ", in state, navigation.Target.Properties, appendTargetColumn);
  }

  private readonly struct AppendColumns : INavigationVisitor<int>
  {
    private readonly StringBuilder _stringBuilder;

    public AppendColumns(StringBuilder stringBuilder)
    {
      _stringBuilder = stringBuilder;
    }

    public void Visit(INavigation navigation, int index)
    {
      navigation.Accept(in this, in index);
    }

    public void VisitNode(INavigationNode navigation, in int index)
    {
      AppendNodeColumns(_stringBuilder, in index, navigation);
    }

    public void VisitLeaf(INavigationLeaf navigation, in int index)
    {
      AppendLeafColumns(_stringBuilder, in index, navigation, AppendTargetAliasedColumn);
    }
  }

  private readonly struct AppendUnaliasedColumns : INavigationVisitor<int>
  {
    private readonly StringBuilder _stringBuilder;

    public AppendUnaliasedColumns(StringBuilder stringBuilder)
    {
      _stringBuilder = stringBuilder;
    }

    public void Visit(INavigation navigation, int index)
    {
      navigation.Accept(in this, in index);
    }

    public void VisitNode(INavigationNode navigation, in int index)
    {
      AppendNodeColumns(_stringBuilder, in index, navigation);
    }

    public void VisitLeaf(INavigationLeaf navigation, in int index)
    {
      AppendLeafColumns(_stringBuilder, in index, navigation, AppendTargetUnaliasedColumn);
    }
  }

  private readonly struct AppendTarget : INavigationVisitor<int>
  {
    private readonly StringBuilder _stringBuilder;

    public AppendTarget(StringBuilder stringBuilder)
    {
      _stringBuilder = stringBuilder;
    }

    public void Visit(INavigation navigation, int index)
    {
      navigation.Accept(in this, in index);
    }

    public void VisitNode(INavigationNode navigation, in int index)
    {
      _stringBuilder
        .AppendLine("(")
        .Append("SELECT ")
        .AppendJoin(", ", in index, navigation.Target.Properties, AppendTargetColumn)
        .Append(", ")
        .AppendJoin(", ", navigation.Children, AppendChildrenColumns)
        .AppendLine()
        .Append("FROM [")
        .Append(navigation.Target.TableName)
        .AppendLine("] AS t");

      foreach (INavigation child in navigation.Children)
      {
        _stringBuilder.Append("LEFT JOIN ");
        new AppendTarget(_stringBuilder).Visit(child, child.Index);
        _stringBuilder
          .Append(" AS t")
          .Append(child.Index)
          .Append(" ON ");
        new AppendJoinCondition(_stringBuilder).Visit(child, child.Index);
      }

      _stringBuilder
        .AppendLine()
        .Append(")");

      static void AppendTargetColumn(StringBuilder stringBuilder, in int index, IPropertyModel property)
      {
        stringBuilder
          .Append("t.[")
          .Append(property.ColumnName)
          .Append("] AS [")
          .Append(property.ColumnName)
          .Append('_')
          .Append(index)
          .Append(']');
      }
    }

    public void VisitLeaf(INavigationLeaf navigation, in int index)
    {
      _stringBuilder
        .Append("[")
        .Append(navigation.Target.TableName)
        .Append(']');
    }
  }

  public readonly struct AppendJoinCondition : INavigationVisitor<int>
  {
    private readonly StringBuilder _stringBuilder;

    public AppendJoinCondition(StringBuilder stringBuilder)
    {
      _stringBuilder = stringBuilder;
    }

    public void Visit(INavigation navigation, int index)
    {
      navigation.Accept(in this, in index);
    }

    public void VisitNode(INavigationNode navigation, in int index)
    {
      IndexPair state = new(index, navigation.Index);

      _stringBuilder.AppendJoin(", ", in state, navigation.Model.Keys, AppendCondition);

      static void AppendCondition(StringBuilder stringBuilder, in IndexPair state, IKeyPair pair)
      {
        stringBuilder
          .Append("t.[")
          .Append(pair.FromProperty.ColumnName)
          .Append("] = t")
          .Append(state.Index)
          .Append(".[")
          .Append(pair.ToProperty.ColumnName)
          .Append('_')
          .Append(state.NavigationIndex)
          .Append(']');
      }
    }

    public void VisitLeaf(INavigationLeaf navigation, in int index)
    {
      _stringBuilder.AppendJoin(", ", in index, navigation.Model.Keys, AppendCondition);

      static void AppendCondition(StringBuilder stringBuilder, in int index, IKeyPair pair)
      {
        stringBuilder
          .Append("t.[")
          .Append(pair.FromProperty.ColumnName)
          .Append("] = t")
          .Append(index)
          .Append(".[")
          .Append(pair.ToProperty.ColumnName)
          .Append(']');
      }
    }
  }

  private readonly record struct IndexPair(int Index, int NavigationIndex);
}
