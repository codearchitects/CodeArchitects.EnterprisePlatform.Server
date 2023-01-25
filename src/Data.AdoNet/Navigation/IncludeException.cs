using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

/// <summary>
/// The exception that is thrown when an invalid include expression or path is encountered.
/// </summary>
[Serializable]
public sealed class IncludeException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="IncludeException"/> class with default error message.
  /// </summary>
  public IncludeException()
    : base("Invalid include expression.")
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="IncludeException"/> class with a specified error message.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public IncludeException(string message)
    : base(message)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="IncludeException"/> class with a specified error message and the reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
  public IncludeException(string message, Exception inner)
    : base(message, inner)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="IncludeException"/> class with serialized data.
  /// </summary>
  /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
  /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
  private IncludeException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
