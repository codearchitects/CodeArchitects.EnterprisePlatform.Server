using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Controls how the provider behaves when an operation needs a MongoDB transaction.
/// </summary>
/// <remarks>
/// MongoDB transactions require a replica set or a sharded cluster; they are not available
/// on a standalone server.
/// </remarks>
[Experimental]
public enum TransactionMode
{
  /// <summary>
  /// Default. Fails with <see cref="TransactionsNotSupportedException"/> when the topology
  /// does not support transactions.
  /// </summary>
  Required = 0,

  /// <summary>
  /// Uses transactions when available, otherwise runs the operations without atomicity
  /// and logs a warning.
  /// </summary>
  WhenSupported = 1,

  /// <summary>
  /// Never uses transactions. Intended for local development against a standalone instance.
  /// </summary>
  Disabled = 2
}
