using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Features.Concurrency;

internal interface IConcurrencyContext
{
  void CreateToken(object entity, IAccessibleColumnModel concurrencyColumn);
  object GetToken(object entity);
  void Accept();
  void Clear();
}
