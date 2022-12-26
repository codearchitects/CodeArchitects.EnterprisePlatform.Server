namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class EntityModel<TEntity, TKey> : EntityModel, IEntityModel<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EntityModel(int id, InitializerModel initializer, PrimaryKeyModel<TKey> primaryKey)
    : base(id, initializer)
  {
    PrimaryKey = primaryKey;
  }

  public override Type Type => typeof(TEntity);

  public new PrimaryKeyModel<TKey> PrimaryKey { get; }

  protected override PrimaryKeyModel PrimaryKeyCore => PrimaryKey;

  IPrimaryKeyModel<TKey> IEntityModel<TEntity, TKey>.PrimaryKey => PrimaryKey;
}
