using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Visitors;

namespace CodeArchitects.Platform.Data.AdoNet.Executor;

internal partial class Executor<TDbCommand>
{
  public async Task ExecuteUpdateManyAsync<TEntity, TKey>(TDbCommand command, IEnumerable<TEntity> entities, IEntityModel<TEntity, TKey> model, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>
  {
    ExecuteUpdateGraphVisitor visitor = new(this, command);
    foreach (TEntity entity in entities)
    {
      if (model.ConcurrencyToken is IAccessibleColumnModel concurrencyColumn)
      {
        _concurrencyContext.CreateToken(entity, concurrencyColumn);
      }

      await Graph.VisitAsync(entity, model, visitor, cancellationToken);
    }
  }
}
