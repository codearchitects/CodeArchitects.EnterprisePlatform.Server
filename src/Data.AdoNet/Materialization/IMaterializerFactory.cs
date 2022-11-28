namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializerFactory
{
  IMaterializer<TEntity, TKey> CreateMaterializer<TEntity, TKey>(IMaterializerHub hub)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
