using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Sql.Select;

namespace CodeArchitects.Platform.Data.AdoNet.Sql;

internal class SqlTextBuilder : ISqlTextBuilder // TODO: Support multiple database providers
{
  private readonly ISqlTextCache _cache;

  public SqlTextBuilder(ISqlTextCache cache)
  {
    _cache = cache;
  }

  public string BuildSelectText(INavigationRoot root)
  {
    IEntityModel entity = root.Entity;
    IReadOnlyCollection<INavigation> navigations = root.Navigations;
    NavigationCacheKey key = new NavigationCacheKey(entity, navigations);

    if (_cache.TryGetSelectText(key, out string? text))
      return text;

    SelectStringBuilder stringBuilder = new();

    stringBuilder.Append("SELECT ");
    stringBuilder.AppendJoin(", ", entity.Properties, AppendTargetColumn);
    stringBuilder.Append(", ");
    stringBuilder.AppendChildrenColumns(navigations);
    stringBuilder.AppendLine();
    stringBuilder.AppendLine("FROM (");
    stringBuilder.Append("SELECT ");
    stringBuilder.AppendJoin(", ", entity.Properties, AppendColumn);
    stringBuilder.AppendLine();
    stringBuilder.Append("FROM [");
    stringBuilder.Append(entity.TableName);
    stringBuilder.AppendLine("]");
    stringBuilder.Append("WHERE ");
    stringBuilder.AppendJoin(" AND ", entity.PrimaryKey.Properties, AppendWhereCondition);
    stringBuilder.AppendLine();
    stringBuilder.Append(") AS t");

    foreach (INavigation child in navigations)
    {
      stringBuilder.AppendLine();
      stringBuilder.Append("LEFT JOIN ");
      stringBuilder.AppendTarget(child);
      stringBuilder.Append(" AS t");
      stringBuilder.Append(child.Index);
      stringBuilder.Append(" ON ");
      stringBuilder.AppendJoinConditions(child);
    }

    text = stringBuilder.ToString();
    _cache.AddSelectText(key, text);

    return text;

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
}
