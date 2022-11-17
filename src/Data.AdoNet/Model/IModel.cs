using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IModel
{
  bool TryGetEntity(Type entityType, [NotNullWhen(true)] out IEntityModel entity);
}
