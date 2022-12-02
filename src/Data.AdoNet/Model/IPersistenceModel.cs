using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IPersistenceModel
{
  bool TryGetEntity(Type entityType, [NotNullWhen(true)] out IEntityModel entity);

  bool TryGetEntity<TEntity, TKey>([NotNullWhen(true)] out IEntityModel<TEntity, TKey> entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
