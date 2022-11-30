using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IRowReaderProvider
{
  RowReader GetRowReader(IEntityModel entity);
}
