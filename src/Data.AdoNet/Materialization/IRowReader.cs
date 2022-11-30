using CodeArchitects.Platform.Data.AdoNet.Navigation;
using System.Data;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal interface IRowReader
{
  object? ReadRow(IDataReader reader, ref int offset, IMaterializerHub hub, IReadOnlyCollection<INavigation> navigations);
}
