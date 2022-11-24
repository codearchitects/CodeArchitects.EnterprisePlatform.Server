using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Data.AdoNet.Sql;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

internal class CommandBuilder<TEntity, TKey> : ICommandBuilder<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  private readonly ISqlTextBuilder _textBuilder;
  private readonly IEntityModel _entity;

  public CommandBuilder(ISqlTextBuilder textBuilder, IEntityModel entity)
  {
    _textBuilder = textBuilder;
    _entity = entity;
  }

  public void BuildSelectCommand(DbCommand command, TKey key, INavigationRoot navigation)
  {
    throw new NotImplementedException();
  }
}
