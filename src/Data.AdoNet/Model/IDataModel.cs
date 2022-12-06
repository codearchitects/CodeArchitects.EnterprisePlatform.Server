using CodeArchitects.Platform.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IDataModel
{
  IReadOnlyCollection<IEntityModel> Entities { get; }

  bool TryGetEntity(string entityName, [NotNullWhen(true)] out IEntityModel? entity);

  bool TryGetEntity<TEntity, TKey>(string entityName, [NotNullWhen(true)] out IEntityModel<TEntity, TKey>? entity)
    where TEntity : class
    where TKey : IEquatable<TKey>;
}
