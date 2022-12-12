using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IRowReaderProvider
{
  IRowReader GetRowReader(IEntityModel entity);
}
