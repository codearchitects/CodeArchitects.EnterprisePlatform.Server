using System;

namespace CodeArchitects.Platform.Infrastructure.Messaging;

[AttributeUsage(AttributeTargets.Class)]
public class MessageAttribute : Attribute
{
  public MessageAttribute()
  {
  }

  public MessageAttribute(string messageName)
  {
    MessageName = messageName;
  }

  public string? MessageName { get; }
}
