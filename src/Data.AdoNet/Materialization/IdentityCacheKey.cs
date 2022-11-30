using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal readonly record struct IdentityCacheKey(IEntityModel Model, object Key);