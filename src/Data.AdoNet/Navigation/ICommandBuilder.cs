using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

internal interface INewCommandBuilder
{
  string BuildSelectCommand();
}

internal interface INavigation
{
  void WriteSql(StringBuilder stringBuilder);
  void AppendPropertyList(StringBuilder stringBuilder);
}

internal interface INavigationModel
{
  bool IsOnDependent { get; }
  IEntityModel From { get; }
  IEntityModel To { get; }
  IReadOnlyList<IKeyPair> Keys { get; }
}

internal interface IKeyPair
{
  IPropertyModel PrimaryKey { get; }
  IPropertyModel ForeignKey { get; }
}

class NavigationLeaf : INavigation
{
  private readonly INavigationModel _navigation;
  private readonly string _alias;

  public NavigationLeaf(INavigationModel navigation, string alias)
  {
    _navigation = navigation;
    _alias = alias;
  }

  public void AppendPropertyList(StringBuilder stringBuilder)
  {
    stringBuilder.AppendJoin(", ", in _alias, _navigation.To.Properties, AppendMainEntityPropertyList);
  }

  public void WriteSql(StringBuilder stringBuilder)
  {
    NavigationLeaf @this = this;
    stringBuilder
      .Append('[')
      .Append(_navigation.To.Name)
      .Append("] AS ")
      .Append(_alias)
      .Append(" ON ")
      .AppendJoin(", ", in @this, _navigation.Keys, AppendJoinPredicates);
  }

  private static void AppendMainEntityPropertyList(StringBuilder stringBuilder, in string alias, IPropertyModel property)
  {
    stringBuilder
      .Append(alias)
      .Append(".[")
      .Append(property.Name)
      .Append("]");
  }

  private static void AppendJoinPredicates(StringBuilder stringBuilder, in NavigationLeaf @this, IKeyPair pair)
  {
    IPropertyModel first;
    IPropertyModel second;

    if (@this._navigation.IsOnDependent) // TODO: Polymorphism
    {
      first = pair.PrimaryKey;
      second = pair.ForeignKey;
    }
    else
    {
      first = pair.ForeignKey;
      second = pair.PrimaryKey;
    }

    stringBuilder
      .Append("t.[")
      .Append(first.Name)
      .Append("] = ")
      .Append(@this._alias)
      .Append(".[")
      .Append(second.Name)
      .Append("]");
  }
}

class NavigationNode : INavigation
{
  private readonly INavigationModel _navigation;
  private readonly IReadOnlyList<INavigation> _navigations;
  private readonly string _alias;

  public NavigationNode(INavigationModel navigation, IReadOnlyList<INavigation> navigations, string alias)
  {
    _navigation = navigation;
    _navigations = navigations;
    _alias = alias;
  }

  public void AppendPropertyList(StringBuilder stringBuilder)
  {
    stringBuilder.AppendJoin(", ", in _alias, _navigation.To.Properties, AppendMainEntityPropertyList);
  }

  public void WriteSql(StringBuilder stringBuilder)
  {
    NavigationNode @this = this;
    stringBuilder
      .Append('[')
      .Append(_navigation.To.Name)
      .Append("] AS ")
      .Append(_alias)
      .Append(" ON ")
      .AppendJoin(", ", in @this, _navigation.Keys, AppendJoinPredicates);
  }

  private static void AppendMainEntityPropertyList(StringBuilder stringBuilder, in string alias, IPropertyModel property)
  {
    stringBuilder
      .Append(alias)
      .Append(".[")
      .Append(property.Name)
      .Append("]");
  }

  private static void AppendJoinPredicates(StringBuilder stringBuilder, in NavigationNode @this, IKeyPair pair)
  {
    IPropertyModel first;
    IPropertyModel second;

    if (@this._navigation.IsOnDependent) // TODO: Polymorphism
    {
      first = pair.PrimaryKey;
      second = pair.ForeignKey;
    }
    else
    {
      first = pair.ForeignKey;
      second = pair.PrimaryKey;
    }

    stringBuilder
      .Append("t.[")
      .Append(first.Name)
      .Append("] = ")
      .Append(@this._alias)
      .Append(".[")
      .Append(second.Name)
      .Append("]");
  }
}

class NewCommandBuilder : INewCommandBuilder
{
  private readonly List<INavigation> _navigations;
  private readonly IEntityModel _entity;

  public NewCommandBuilder(IEntityModel entity)
  {
    _navigations = new();
    _entity = entity;
  }

  public void AddNavigation(INavigation navigation)
  {
    _navigations.Add(navigation);
  }

  public string BuildSelectCommand()
  {
    StringBuilder stringBuilder = new();

    stringBuilder
      .Append("SELECT ")
      .AppendJoin(", ", _entity.Properties, AppendMainEntityPropertyList)
      .Append(", ")
      .AppendJoin(", ", _navigations, static (stringBuilder, navigation) => navigation.AppendPropertyList(stringBuilder))
      .AppendLine()
      .AppendLine("FROM (");

    BuildSelectTextNoJoins(stringBuilder);

    stringBuilder
      .AppendLine(") AS t")
      .Append("LEFT JOIN ")
      .AppendJoin("\nLEFT JOIN ", _navigations, static (stringBuilder, navigation) => navigation.WriteSql(stringBuilder));

    return stringBuilder.ToString();
  }

  private void BuildSelectTextNoJoins(StringBuilder stringBuilder)
  {
    stringBuilder
      .Append("SELECT TOP(1) ")
      .AppendJoin(", ", _entity.Properties, AppendPropertyList)
      .AppendLine()
      .Append("FROM [")
      .Append(_entity.Name)
      .AppendLine("]")
      .Append("WHERE ")
      .AppendJoin(" AND ", _entity.PrimaryKey.Properties, AppendWherePredicates);
  }

  private static void AppendMainEntityPropertyList(StringBuilder stringBuilder, IPropertyModel property)
  {
    stringBuilder
      .Append("t.[")
      .Append(property.Name)
      .Append("]");
  }

  private static void AppendPropertyList(StringBuilder stringBuilder, IPropertyModel property)
  {
    stringBuilder
      .Append("[")
      .Append(property.Name)
      .Append("]");
  }

  private static void AppendWherePredicates(StringBuilder stringBuilder, IPropertyModel property)
  {
    stringBuilder
      .Append('[')
      .Append(property.Name)
      .Append("] = @p")
      .Append(property.Index);
  }
}
