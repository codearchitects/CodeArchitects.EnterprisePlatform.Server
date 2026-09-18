using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Resolved provider options, shared between the runtime and the dependency-injection assembly.
/// </summary>
internal sealed class MongoDBOptions
{
  /// <summary>
  /// How the provider behaves when an operation needs a transaction. Defaults to
  /// <see cref="TransactionMode.Required"/>.
  /// </summary>
  public TransactionMode TransactionMode { get; set; } = TransactionMode.Required;

  /// <summary>
  /// Read/write concern and timeout applied to the transactions opened by the unit of work.
  /// </summary>
  public TransactionOptions? TransactionOptions { get; set; }
}
