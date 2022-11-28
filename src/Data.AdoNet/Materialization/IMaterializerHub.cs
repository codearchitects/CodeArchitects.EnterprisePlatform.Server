using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializerHub : IIdentityCollectionFactory
{
  IMaterializer<TEntity, TKey> GetMaterializer<TEntity, TKey>(IEntityModel target)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
