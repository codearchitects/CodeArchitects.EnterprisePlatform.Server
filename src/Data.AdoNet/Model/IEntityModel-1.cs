using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IEntityModel<TEntity, TKey> : IEntityModel
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  new IPrimaryKeyModel<TKey> PrimaryKey { get; }
}
