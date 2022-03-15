using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Messaging;

public class InvalidMessageTypeException : Exception
{
  public InvalidMessageTypeException(Type messageType)
  {
    MessageType = messageType;
  }

  public Type MessageType { get; }
}
