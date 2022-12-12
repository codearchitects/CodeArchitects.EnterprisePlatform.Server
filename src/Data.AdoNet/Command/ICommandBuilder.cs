using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ICommandBuilder
{
  void BuildSelectCommand<TEntity, TKey>(IDbCommand command, TKey key, in NavigationSpec<TEntity, TKey> spec)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  void BuildInsertCommand(IDbCommand command, object node, IEntityModel model, in NavigationContext context);

  void BuildUpdateCommand(IDbCommand command, object node, IEntityModel model, in NavigationContext context);

  void BuildDeleteCommand(IDbCommand command, object node, IEntityModel model);

  void BuildDeleteCommand<TEntity, TKey>(IDbCommand command, TKey key, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
