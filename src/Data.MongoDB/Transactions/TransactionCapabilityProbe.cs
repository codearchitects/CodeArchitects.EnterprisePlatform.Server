using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB.Transactions;

/// <summary>
/// Detects the deployment topology through the <c>hello</c> handshake command.
/// </summary>
/// <remarks>
/// The handshake is part of the MongoDB wire protocol, so it is stable across server and driver
/// versions — unlike the driver's internal cluster description, which was reorganized between
/// 2.x and 3.x. The command runs lazily on first use rather than at registration time, so that
/// composing the container never depends on the database being reachable.
/// </remarks>
internal sealed class TransactionCapabilityProbe : ITransactionCapabilityProbe
{
  private const int Unknown = 0;
  private const int Supported = 1;
  private const int Unsupported = 2;

  private readonly IMongoDatabase _database;
  private readonly ILogger _logger;

  private int _state = Unknown;

  public TransactionCapabilityProbe(IMongoDatabase database, ILogger<TransactionCapabilityProbe> logger)
  {
    _database = database;
    _logger = logger;
  }

  public bool AreTransactionsSupported()
  {
    if (TryGetCached(out bool supported))
      return supported;

    // 'hello' only exists from server 4.4.2 onwards, so fall back to the legacy handshake.
    BsonDocument? response = TryRunCommand("hello") ?? TryRunCommand("isMaster");

    return Resolve(response);
  }

  public async Task<bool> AreTransactionsSupportedAsync(CancellationToken cancellationToken)
  {
    if (TryGetCached(out bool supported))
      return supported;

    BsonDocument? response =
      await TryRunCommandAsync("hello", cancellationToken) ??
      await TryRunCommandAsync("isMaster", cancellationToken);

    return Resolve(response);
  }

  private bool TryGetCached(out bool supported)
  {
    int state = Volatile.Read(ref _state);
    supported = state == Supported;

    return state != Unknown;
  }

  private bool Resolve(BsonDocument? response)
  {
    bool? supported = Interpret(response);

    // Only a definitive answer is cached: a transient one is asked again on the next call.
    if (supported is null)
      return false;

    Interlocked.CompareExchange(ref _state, supported.Value ? Supported : Unsupported, Unknown);
    return supported.Value;
  }

  /// <summary>
  /// Returns <c>true</c> or <c>false</c> when the topology is known, <c>null</c> when it cannot be
  /// determined yet.
  /// </summary>
  private bool? Interpret(BsonDocument? response)
  {
    if (response is not null)
    {
      // A replica set member reports 'setName'; a mongos router reports msg == 'isdbgrid'.
      bool supported =
        response.Contains("setName") ||
        (response.TryGetValue("msg", out BsonValue? message) && message == "isdbgrid");

      // A mongod started with --replSet before rs.initiate reports 'isreplicaset' but no 'setName':
      // it is not a standalone server, and it will support transactions once the set is initiated.
      if (!supported &&
          response.TryGetValue("isreplicaset", out BsonValue? isReplicaSet) && isReplicaSet.ToBoolean())
      {
        _logger.LogWarning(
          "The MongoDB server is a replica set member that has not been initiated yet (rs.initiate): " +
          "transactions are not available until it is.");

        return null;
      }

      _logger.LogDebug(
        "MongoDB topology handshake completed: transactions are {State}.",
        supported ? "supported" : "not supported");

      return supported;
    }

    // When in doubt, report unsupported: TransactionMode.Required will then fail explicitly
    // instead of silently running without atomicity. The next call probes again.
    _logger.LogWarning(
      "Could not determine the MongoDB topology: transactions will be treated as unsupported.");

    return null;
  }

  private BsonDocument? TryRunCommand(string command)
  {
    try
    {
      return _database.RunCommand<BsonDocument>(new BsonDocument(command, 1));
    }
    catch (MongoException exception)
    {
      _logger.LogDebug(exception, "The MongoDB '{Command}' command failed.", command);
      return null;
    }
  }

  private async Task<BsonDocument?> TryRunCommandAsync(string command, CancellationToken cancellationToken)
  {
    try
    {
      return await _database.RunCommandAsync<BsonDocument>(
        new BsonDocument(command, 1), cancellationToken: cancellationToken);
    }
    catch (MongoException exception)
    {
      _logger.LogDebug(exception, "The MongoDB '{Command}' command failed.", command);
      return null;
    }
  }
}
