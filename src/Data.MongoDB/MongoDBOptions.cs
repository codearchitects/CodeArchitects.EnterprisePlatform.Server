using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Resolved provider options, shared between the runtime and the dependency-injection assembly.
/// </summary>
internal sealed class MongoDBOptions
{
  /// <summary>
  /// The name of the database the provider works against.
  /// </summary>
  public string DatabaseName { get; set; } = default!;

  /// <summary>
  /// Read/write concern and read preference applied to the database.
  /// </summary>
  public MongoDatabaseSettings? DatabaseSettings { get; set; }

  /// <summary>
  /// How the provider behaves when an operation needs a transaction. Defaults to
  /// <see cref="TransactionMode.Required"/>.
  /// </summary>
  public TransactionMode TransactionMode { get; set; } = TransactionMode.Required;

  /// <summary>
  /// Read/write concern and timeout applied to the transactions opened by the unit of work.
  /// </summary>
  public TransactionOptions? TransactionOptions { get; set; }

  /// <summary>
  /// How <see cref="Guid"/> values are stored. Driver 3.x has no implicit default, so the
  /// provider always sets one.
  /// </summary>
  public GuidRepresentation GuidRepresentation { get; set; } = GuidRepresentation.Standard;
}
