using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ICommandBuilder<TDbCommand>
  where TDbCommand : IDbCommand
{
  void BuildFindCommand<TEntity, TKey>(TDbCommand command, TKey key, in NavigationSpec<TEntity, TKey> spec)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  void BuildInsertCommand(TDbCommand command, object node, IEntityModel model, in NavigationContext context);

  void BuildUpdateCommand(TDbCommand command, object node, IEntityModel model, in NavigationContext context);

  void BuildUpsertCommand(TDbCommand command, object node, IEntityModel model);

  void BuildRemoveCommand(TDbCommand command, object node, IEntityModel model);

  void BuildRemoveCommand<TEntity, TKey>(TDbCommand command, TKey key, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
