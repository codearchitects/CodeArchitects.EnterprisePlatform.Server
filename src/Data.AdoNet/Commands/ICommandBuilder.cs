using System.Data.Common;

namespace CodeArchitects.Platform.Data.AdoNet.Commands;

internal interface ICommandBuilder<TEntity, TKey>
  where TEntity : class
  where TKey : IEquatable<TKey>
{
  void BuildSelectCommand(DbCommand command, TKey key, IReadOnlyCollection<string> paths);
}
