namespace CodeArchitects.Platform.Data.MongoDB.Transactions;

/// <summary>
/// Reports whether the connected deployment supports multi-document transactions.
/// </summary>
internal interface ITransactionCapabilityProbe
{
  /// <summary>
  /// Returns <c>true</c> for a replica set or a sharded cluster, <c>false</c> for a standalone
  /// server. The result is determined once and cached: the kind of deployment does not change
  /// during the lifetime of the process.
  /// </summary>
  bool AreTransactionsSupported();
}
