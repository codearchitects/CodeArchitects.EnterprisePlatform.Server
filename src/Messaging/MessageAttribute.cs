namespace CodeArchitects.Platform.Messaging;

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
