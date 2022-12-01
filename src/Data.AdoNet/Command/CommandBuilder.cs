using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal class CommandBuilder : ICommandBuilder // TODO: Support multiple database providers
{
  private readonly ISqlTextBuilder _sqlBuilder;

  public CommandBuilder(ISqlTextBuilder sqlBuilder)
  {
    _sqlBuilder = sqlBuilder;
  }

  public void BuildSelectCommand<TEntity, TKey>(DbCommand command, TKey key, NavigationSpec<TEntity, TKey> spec)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    IEntityModel<TEntity, TKey> model = spec.Entity;

    command.CommandText = _sqlBuilder.BuildSelectText(spec);

    if (model.PrimaryKey.IsComposite)
    {
      for (int i = 0; i < model.PrimaryKey.Properties.Count; i++)
      {
        IPrimaryKeyPropertyModel property = model.PrimaryKey.Properties[i];
        CreateParameter(command, $"p{property.KeyIndex}", model.PrimaryKey.GetKeyComponent(key, i));
      }
    }
    else
    {
      CreateParameter(command, $"p{model.PrimaryKey.Properties[0].KeyIndex}", key);
    }
  }

  public void BuildInsertCommand(DbCommand command, object node, IEntityModel model, in NavigationContext context)
  {
    command.Parameters.Clear();
    throw new NotImplementedException();
  }

  public void BuildUpdateCommand(DbCommand command, object node, IEntityModel model, in NavigationContext context)
  {
    command.Parameters.Clear();
    throw new NotImplementedException();
  }

  public void BuildDeleteCommand<TEntity, TKey>(DbCommand command, TKey key, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    if (model.PrimaryKey.IsComposite)
    {
      for (int i = 0; i < model.PrimaryKey.Properties.Count; i++)
      {
        IPrimaryKeyPropertyModel property = model.PrimaryKey.Properties[i];
        CreateParameter(command, $"p{property.KeyIndex}", model.PrimaryKey.GetKeyComponent(key, i));
      }
    }
    else
    {
      CreateParameter(command, $"p{model.PrimaryKey.Properties[0].KeyIndex}", key);
    }
    throw new NotImplementedException();
  }

  private static void CreateParameter(DbCommand command, string name, object value)
  {
    DbParameter parameter = command.CreateParameter();
    parameter.ParameterName = name;
    parameter.Value = value;
    command.Parameters.Add(parameter);
  }
}
