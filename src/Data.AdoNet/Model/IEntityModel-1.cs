namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IEntityModel<TEntity, TKey> : IEntityModel
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  new IPrimaryKeyModel<TKey> PrimaryKey { get; }
}
