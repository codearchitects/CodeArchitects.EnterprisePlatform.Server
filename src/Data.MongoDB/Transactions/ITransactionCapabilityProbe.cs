namespace CodeArchitects.Platform.Data.MongoDB.Transactions;

/// <summary>
/// Reports whether the connected deployment supports multi-document transactions.
/// </summary>
internal interface ITransactionCapabilityProbe
{
  /// <summary>
  /// Returns <c>true</c> for a replica set or a sharded cluster, <c>false</c> for a standalone
  /// server. A definitive result is determined once and cached: the kind of deployment does not
  /// change during the lifetime of the process. A replica set member that has not been initiated
  /// yet, or a failed handshake, returns <c>false</c> without caching, so the next call asks again.
  /// </summary>
  bool AreTransactionsSupported();

  /// <inheritdoc cref="AreTransactionsSupported"/>
  /// <param name="cancellationToken">A token to cancel the handshake.</param>
  Task<bool> AreTransactionsSupportedAsync(CancellationToken cancellationToken);
}
