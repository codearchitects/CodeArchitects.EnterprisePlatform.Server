using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializer<TEntity>
  where TEntity : class
{
  Task<TEntity?> MaterializeAsync(DbDataReader dataReader);
}
