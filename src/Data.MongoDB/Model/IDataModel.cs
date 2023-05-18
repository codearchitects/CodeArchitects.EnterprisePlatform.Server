using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.MongoDB.Model;

internal interface IDataModel
{
  IReadOnlyCollection<IEntityModel> Entities { get; }

  bool TryGetEntity(Type entityType, [NotNullWhen(true)] out IEntityModel? entity);
}
