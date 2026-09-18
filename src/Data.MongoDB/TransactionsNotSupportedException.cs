using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Thrown when an operation requires a MongoDB transaction but the current deployment
/// cannot provide one.
/// </summary>
[Experimental]
public class TransactionsNotSupportedException : NotSupportedException
{
  private const string DefaultMessage =
    "The operation requires a MongoDB transaction, but the server is neither a replica set nor a " +
    "sharded cluster. Use a compatible deployment, or configure " +
    "UseTransactions(TransactionMode.WhenSupported) or UseTransactions(TransactionMode.Disabled).";

  /// <summary>
  /// Initializes a new instance of the <see cref="TransactionsNotSupportedException"/> class.
  /// </summary>
  public TransactionsNotSupportedException()
    : base(DefaultMessage)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TransactionsNotSupportedException"/> class.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public TransactionsNotSupportedException(string message)
    : base(message)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TransactionsNotSupportedException"/> class.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  /// <param name="innerException">The exception that caused this one.</param>
  public TransactionsNotSupportedException(string message, Exception innerException)
    : base(message, innerException)
  {
  }
}
