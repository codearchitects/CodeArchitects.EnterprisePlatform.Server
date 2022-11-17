using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Common.Exceptions;

/// <summary>
/// The exception that is thrown when one of the type arguments provided to a method is not valid.
/// </summary>
[Serializable]
public class TypeArgumentException : SystemException, ISerializable
{
  public TypeArgumentException()
    : base("Invalid type argument.")
  {
  }

  public TypeArgumentException(string message)
    : base(message)
  {
  }

  public TypeArgumentException(string message, Exception inner)
    : base(message, inner)
  {
  }

  public TypeArgumentException(string message, string paramName)
    : base($"{message} (Type parameter '{paramName}')")
  {
    ParamName = paramName;
  }

  protected TypeArgumentException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public virtual string? ParamName { get; }
}