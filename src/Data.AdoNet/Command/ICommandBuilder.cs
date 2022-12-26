using CodeArchitects.Platform.Data.AdoNet.Executor;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Command;

internal interface ICommandBuilder<in TDbCommand>
  where TDbCommand : IDbCommand
{
  void BuildFindCommand<TEntity, TKey>(TDbCommand command, TKey key, INavigationRoot<TEntity, TKey> root)
    where TEntity : class
    where TKey : IEquatable<TKey>;

  void BuildInsertCommand(TDbCommand command, object entity, IEntityModel model, in NavigationContext context);

  void BuildUpdateCommand(TDbCommand command, object entity, IEntityModel model, in NavigationContext context);

  void BuildUpsertCommand(TDbCommand command, object entity, IEntityModel model);

  void BuildRemoveCommand(TDbCommand command, object entity, IEntityModel model);

  void BuildRemoveCommand<TEntity, TKey>(TDbCommand command, TKey key, IEntityModel<TEntity, TKey> model)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
