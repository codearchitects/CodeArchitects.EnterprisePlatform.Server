using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Text;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

internal class SqlTextBuilder : ISqlTextBuilder // TODO: Support multiple database providers
{
  public string BuildSelectText(IEntityModel entity, IReadOnlyCollection<string> paths)
  {
    return paths.Count == 0
      ? BuildSelectTextNoJoins(entity)
      : BuildSelectTextWithJoins(entity, paths);
  }

  private string BuildSelectTextNoJoins(IEntityModel entity)
  {
    return new StringBuilder("SELECT TOP(1) ")
      .AppendJoin(", ", GetPropertyList(entity.Properties))
      .Append(" FROM [")
      .Append(entity.Name)
      .Append("] AS t WHERE ")
      .AppendJoin(" AND ", GetWherePredicates(entity.PrimaryKey.Properties))
      .ToString();

    static IEnumerable<string> GetPropertyList(IEnumerable<IPropertyModel> properties)
    {
      return properties.Select(property => $"t.[{property.Name}]");
    }

    static IEnumerable<string> GetWherePredicates(IEnumerable<IPrimaryKeyPropertyModel> properties)
    {
      return properties.Select(property => $"t.[{property.Name}] = @p{property.Index}");
    }
  }

  private string BuildSelectTextWithJoins(IEntityModel entity, IEnumerable<string> paths)
  {
    throw new NotImplementedException();
  }
}
