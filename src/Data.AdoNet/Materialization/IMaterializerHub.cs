using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializerHub : IIdentityCollectionFactory
{
  object? ReadRow(IDataReader reader, ref int offset, IEntityModel model, IReadOnlyCollection<INavigation> navigations);
  void AddMaterialized(IdentityCacheKey key, object materialized);
  bool TryGetExisting(IdentityCacheKey key, [NotNullWhen(true)] out object? existing);
}