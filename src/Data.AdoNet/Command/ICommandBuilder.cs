using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ICommandBuilder
{
  void BuildSelectCommand<TEntity, TKey>(DbCommand command, TKey key, NavigationSpec<TEntity, TKey> spec)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  void BuildInsertCommand(DbCommand command, object entity, NavigationContext context);

  void BuildUpdateCommand(DbCommand command, object entity, NavigationContext context);

  void BuildDeleteCommand<TEntity, TKey>(DbCommand command, TKey key, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
