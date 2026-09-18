using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

internal interface IStateManager : Data.IStateManager
{
  /// <summary>
  /// The ambient session of the current scope, created lazily on first use. Every operation —
  /// reads included — runs on it, so that they share causal consistency and take part in the
  /// transaction opened by the unit of work.
  /// </summary>
  IClientSessionHandle Session { get; }

  /// <summary>
  /// Records a pending write.
  /// </summary>
  /// <param name="execution">The operation, in both its synchronous and asynchronous form.</param>
  /// <param name="requiresTransaction">
  /// <c>true</c> for operations that are only atomic inside a transaction, such as writes
  /// spanning several documents.
  /// </param>
  void AddExecution(Execution execution, bool requiresTransaction);
}
