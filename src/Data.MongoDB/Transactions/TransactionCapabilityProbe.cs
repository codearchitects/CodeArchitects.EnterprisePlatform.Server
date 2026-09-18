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
    int state = Volatile.Read(ref _state);
    if (state != Unknown)
      return state == Supported;

    bool supported = Probe();

    Interlocked.CompareExchange(ref _state, supported ? Supported : Unsupported, Unknown);
    return supported;
  }

  private bool Probe()
  {
    // 'hello' only exists from server 4.4.2 onwards, so fall back to the legacy handshake.
    if (TryRunCommand("hello", out BsonDocument? response) ||
        TryRunCommand("isMaster", out response))
    {
      // A replica set member reports 'setName'; a mongos router reports msg == 'isdbgrid'.
      bool supported =
        response!.Contains("setName") ||
        (response.TryGetValue("msg", out BsonValue? message) && message == "isdbgrid");

      _logger.LogDebug(
        "MongoDB topology handshake completed: transactions are {State}.",
        supported ? "supported" : "not supported");

      return supported;
    }

    // When in doubt, report unsupported: TransactionMode.Required will then fail explicitly
    // instead of silently running without atomicity.
    _logger.LogWarning(
      "Could not determine the MongoDB topology: transactions will be treated as unsupported.");

    return false;
  }

  private bool TryRunCommand(string command, out BsonDocument? response)
  {
    try
    {
      response = _database.RunCommand<BsonDocument>(new BsonDocument(command, 1));
      return true;
    }
    catch (MongoException exception)
    {
      _logger.LogDebug(exception, "The MongoDB '{Command}' command failed.", command);
      response = null;
      return false;
    }
  }
}
