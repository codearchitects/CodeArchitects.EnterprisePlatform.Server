using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Common.Exceptions;

/// <summary>
/// The exception that is thrown when one of the type arguments provided to a method is not valid.
/// </summary>
[Serializable]
public class TypeArgumentException : SystemException
{
  /// <summary>
  /// Initializes a new instance of the <see cref="TypeArgumentException"/> class with default error message.
  /// </summary>
  public TypeArgumentException()
    : base("Invalid type argument.")
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TypeArgumentException"/> class with a specified error message.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public TypeArgumentException(string message)
    : base(message)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TypeArgumentException"/> class with a specified error message and the reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
  public TypeArgumentException(string message, Exception inner)
    : base(message, inner)
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TypeArgumentException"/> class with a specified error message and the name of the type parameter that causes the exception.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  /// <param name="paramName">The name of the type parameter that caused the exception.</param>
  public TypeArgumentException(string message, string paramName)
    : base($"{message} (Type parameter '{paramName}')")
  {
    ParamName = paramName;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TypeArgumentException"/> class with serialized data.
  /// </summary>
  /// <param name="info">The System.Runtime.Serialization.SerializationInfo that holds the serialized object data about the exception being thrown.</param>
  /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
  protected TypeArgumentException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  /// <summary>
  /// The name of the type parameter that caused the exception.
  /// </summary>
  public virtual string? ParamName { get; }
}