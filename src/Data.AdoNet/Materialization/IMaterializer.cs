using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializer
{
  Task<TEntity?> ReadEntityAsync<TEntity, TKey>(DbDataReader reader, IEntityModel target, IReadOnlyCollection<INavigation> navigations, CancellationToken cancellationToken)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
