using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INavigationSpec
{
  IEntityModel Entity { get; }
  IReadOnlyList<INavigation> Navigations { get; }
}

internal interface INavigation
{
  void AppendColumns(int index, StringBuilder stringBuilder);
  void AppendTarget(int index, StringBuilder stringBuilder);
  void AppendJoinConditions(int index, StringBuilder stringBuilder);
}

internal interface ISubNavigation : INavigation
{
  void AppendUnaliasedColumns(int index, StringBuilder stringBuilder);
}

class NavigationNode : ISubNavigation
{
  private readonly INavigationModel _model;
  private readonly IReadOnlyList<ISubNavigation> _navigations;

  public NavigationNode(INavigationModel model, IReadOnlyList<ISubNavigation> navigations)
  {
    _model = model;
    _navigations = navigations;
  }

  private IEntityModel Entity => _model.To;

  public void AppendColumns(int index, StringBuilder stringBuilder)
  {
    AppendUnaliasedColumns(index, stringBuilder);
  }

  public void AppendUnaliasedColumns(int index, StringBuilder stringBuilder)
  {
    (int, int) state = (index, _model.Id);

    stringBuilder
      .AppendJoin(", ", ref state, Entity.Properties, AppendTargetColumns)
      .Append(", ")
      .AppendJoin(", ", ref index, _navigations, AppendNavigationColumns);

    static void AppendTargetColumns(StringBuilder stringBuilder, ref (int Index, int NavigationId) state, IPropertyModel property)
    {
      stringBuilder
        .Append('t')
        .Append(state.Index)
        .Append(".[")
        .Append(property.ColumnName)
        .Append('_')
        .Append(state.NavigationId)
        .Append(']');
    }

    static void AppendNavigationColumns(StringBuilder stringBuilder, ref int index, ISubNavigation navigation)
    {
      navigation.AppendUnaliasedColumns(index, stringBuilder);
    }
  }

  public void AppendTarget(int index, StringBuilder stringBuilder)
  {
    int navigationId = _model.Id;
    int subIndex = 0;

    stringBuilder
      .AppendLine("(")
      .Append("SELECT ")
      .AppendJoin(", ", ref navigationId, Entity.Properties, AppendTargetColumns)
      .Append(", ")
      .AppendJoin(", ", ref subIndex, _navigations, AppendNavigationColumns)
      .AppendLine()
      .Append("FROM [")
      .Append(Entity.TableName)
      .AppendLine("] AS t");

    for (subIndex = 0; subIndex < _navigations.Count; subIndex++)
    {
      ISubNavigation navigation = _navigations[subIndex];

      stringBuilder.Append("LEFT JOIN ");
      navigation.AppendTarget(subIndex, stringBuilder);
      stringBuilder
        .Append(" AS t")
        .Append(subIndex)
        .Append(" ON ");
      navigation.AppendJoinConditions(subIndex, stringBuilder);
    }

    stringBuilder
      .AppendLine()
      .Append(")");

    static void AppendTargetColumns(StringBuilder stringBuilder, ref int navigationId, IPropertyModel property)
    {
      stringBuilder
        .Append("t.[")
        .Append(property.ColumnName)
        .Append("] AS [")
        .Append(property.ColumnName)
        .Append("_")
        .Append(navigationId)
        .Append(']');
    }

    static void AppendNavigationColumns(StringBuilder stringBuilder, ref int subIndex, ISubNavigation navigation)
    {
      navigation.AppendColumns(subIndex, stringBuilder);
      subIndex++;
    }
  }

  public void AppendJoinConditions(int index, StringBuilder stringBuilder)
  {
    (int, int) state = (index, _model.Id);

    stringBuilder.AppendJoin(", ", ref state, _model.Keys, AppendCondition);

    static void AppendCondition(StringBuilder stringBuilder, ref (int Index, int NavigationId) state, IKeyPair pair)
    {
      stringBuilder
        .Append("t.[")
        .Append(pair.FromProperty.ColumnName)
        .Append("] = t")
        .Append(state.Index)
        .Append(".[")
        .Append(pair.ToProperty.ColumnName)
        .Append('_')
        .Append(state.NavigationId)
        .Append(']');
    }
  }
}

class NavigationLeaf : ISubNavigation
{
  private readonly INavigationModel _model;

  public NavigationLeaf(INavigationModel model)
  {
    _model = model;
  }

  private IEntityModel Entity => _model.To;

  public void AppendColumns(int index, StringBuilder stringBuilder)
  {
    (int, int) state = (index, _model.Id);

    stringBuilder.AppendJoin(", ", ref state, Entity.Properties, AppendTargetColumns);

    static void AppendTargetColumns(StringBuilder stringBuilder, ref (int Index, int NavigationId) state, IPropertyModel property)
    {
      stringBuilder
        .Append('t')
        .Append(state.Index)
        .Append(".[")
        .Append(property.ColumnName)
        .Append("] AS [")
        .Append(property.ColumnName)
        .Append("_")
        .Append(state.NavigationId)
        .Append(']');
    }
  }


  public void AppendUnaliasedColumns(int index, StringBuilder stringBuilder)
  {
    (int, int) state = (index, _model.Id);

    stringBuilder.AppendJoin(", ", ref state, Entity.Properties, AppendTargetColumns);

    static void AppendTargetColumns(StringBuilder stringBuilder, ref (int Index, int NavigationId) state, IPropertyModel property)
    {
      stringBuilder
        .Append('t')
        .Append(state.Index)
        .Append(".[")
        .Append(property.ColumnName)
        .Append("_")
        .Append(state.NavigationId)
        .Append(']');
    }
  }

  public void AppendTarget(int index, StringBuilder stringBuilder)
  {
    stringBuilder
      .Append("[")
      .Append(Entity.TableName)
      .Append(']');
  }

  public void AppendJoinConditions(int index, StringBuilder stringBuilder)
  {
    stringBuilder.AppendJoin(", ", ref index, _model.Keys, AppendCondition);

    static void AppendCondition(StringBuilder stringBuilder, ref int index, IKeyPair pair)
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

class SqlBuilder
{
  public string WriteSelectSql(INavigationSpec spec)
  {
    IEntityModel entity = spec.Entity;
    IReadOnlyList<INavigation> navigations = spec.Navigations;

    StringBuilder stringBuilder = new();
    int index = 0;

    stringBuilder
      .Append("SELECT ")
      .AppendJoin(", ", entity.Properties, static (stringBuilder, property) => stringBuilder
        .Append("t.[")
        .Append(property.ColumnName)
        .Append(']'))
      .Append(", ")
      .AppendJoin(", ", ref index, navigations, AppendNavigationColumns)
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

    for (index = 0; index < navigations.Count; index++)
    {
      INavigation navigation = navigations[index];

      stringBuilder.AppendLine();
      stringBuilder.Append("LEFT JOIN ");
      navigation.AppendTarget(index, stringBuilder);
      stringBuilder
        .Append(" AS t")
        .Append(index)
        .Append(" ON ");
      navigation.AppendJoinConditions(index, stringBuilder);
    }

    return stringBuilder.ToString();
  }

  private static void AppendNavigationColumns(StringBuilder stringBuilder, ref int index, INavigation navigation)
  {
    navigation.AppendColumns(index, stringBuilder);
    index++;
  }
}