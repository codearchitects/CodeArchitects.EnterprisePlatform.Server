using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

internal class SqlTextBuilder : ISqlTextBuilder // TODO: Support multiple database providers
{
  public string BuildSelectText(INavigationPlan plan)
  {
    StringBuilder stringBuilder = new();

    if (plan.Navigations.Count == 0)
    {
      BuildSelectTextNoJoins(stringBuilder, plan.Entity);
    }
    else
    {
      BuildSelectTextWithJoins(stringBuilder, plan);
    }

    return stringBuilder.ToString();
  }

  private static void BuildSelectTextNoJoins(StringBuilder stringBuilder, IEntityModel entity)
  {
    stringBuilder
      .Append("SELECT TOP(1) ")
      .AppendJoin(", ", entity.Properties, AppendPropertyList)
      .AppendLine()
      .Append("FROM [")
      .Append(entity.Name)
      .AppendLine("]")
      .Append("WHERE ")
      .AppendJoin(" AND ", entity.PrimaryKey.Properties, AppendWherePredicates);
  }

  private static void BuildSelectTextWithJoins(StringBuilder stringBuilder, INavigationPlan plan)
  {
    stringBuilder
      .Append("SELECT ")
      .AppendJoin(", ", plan.Properties, AppendPropertyList)
      .AppendLine()
      .AppendLine("FROM (");

    BuildSelectTextNoJoins(stringBuilder, plan.Entity);

    stringBuilder.AppendLine();

    stringBuilder.AppendLine(") AS t");

    foreach (INavigationPlan navigation in plan.Navigations)
    {
      stringBuilder.AppendLine("LEFT JOIN (");
      stringBuilder.AppendLine(")");
    }
  }

  private static void AppendPropertyList(StringBuilder stringBuilder, IPropertyModel property)
  {
    stringBuilder
      .Append('[')
      .Append(property.Name)
      .Append(']');
  }

  private static void AppendWherePredicates(StringBuilder stringBuilder, IPropertyModel property)
  {
    stringBuilder
      .Append('[')
      .Append(property.Name)
      .Append("] = @p")
      .Append(property.Index);
  }

  private static void AppendWherePredicatesWithAlias(StringBuilder stringBuilder, string alias, IPropertyModel property)
  {
    stringBuilder
      .Append(alias)
      .Append(".[")
      .Append(property.Name)
      .Append("] = @p")
      .Append(property.Index);
  }
}
