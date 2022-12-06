namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class EntityModel<TEntity, TKey> : EntityModel, IEntityModel<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  public EntityModel(IInitializerModel initializer, IReadOnlyList<IColumnModel> columns, IPrimaryKeyModel<TKey> primaryKey)
    : base(initializer, columns)
  {
    PrimaryKey = primaryKey;
  }

  public override Type Type => typeof(TEntity);

  public new IPrimaryKeyModel<TKey> PrimaryKey { get; }

  protected override IPrimaryKeyModel PrimaryKeyCore => PrimaryKey;
}
