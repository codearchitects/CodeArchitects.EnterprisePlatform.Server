using CodeArchitects.Platform.Data.AdoNet.Model;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

internal interface ISqlTextBuilder
{
  string BuildSelectText(IEntityModel entity, IReadOnlyCollection<string> paths);
}
