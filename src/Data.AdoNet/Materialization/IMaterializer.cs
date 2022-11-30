using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IMaterializer
{
  Task<object?> ReadEntityAsync(DbDataReader reader, NavigationSpec spec, CancellationToken cancellationToken);
}
