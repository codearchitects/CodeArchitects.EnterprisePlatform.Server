using System.Runtime.Serialization;

namespace System;

[Serializable]
internal class MissingPropertyException : MissingMemberException
{
  public MissingPropertyException()
    : base("Attempted to access a missing property.")
  {
  }

  public MissingPropertyException(string? message)
    : base(message)
  {
  }

  public MissingPropertyException(string message, Exception inner)
    : base(message, inner)
  {
  }

  public MissingPropertyException(string? className, string? propertyName)
  {
    ClassName = className;
    MemberName = propertyName;
  }

  protected MissingPropertyException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }

  public override string Message => ClassName is null
    ? base.Message
    : $"Property '{ClassName}.{MemberName}' not found.";
}