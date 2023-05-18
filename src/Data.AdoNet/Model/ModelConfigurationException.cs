using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// Represents an error that occurs while configuring the data model.
/// </summary>
[Serializable]
public class ModelConfigurationException : Exception
{
  /// <summary>
  /// Creates a new <see cref="ModelConfigurationException"/> with a default error message.
  /// </summary>
  public ModelConfigurationException()
    : base("There was an error during the data model construction.")
  {
  }

  /// <summary>
  /// Creates a new <see cref="ModelConfigurationException"/> with a specified error message.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public ModelConfigurationException(string message)
    : base(message)
  {
  }

  /// <summary>
  /// Creates a new <see cref="ModelConfigurationException"/> with a specified error message and a reference to the inner exception that is the cause of this exception.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  /// <param name="inner">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
  public ModelConfigurationException(string message, Exception inner)
    : base(message, inner)
  {
  }

  /// <summary>
  /// Creates a new <see cref="ModelConfigurationException"/> with serialized data.
  /// </summary>
  /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
  /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
  protected ModelConfigurationException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}