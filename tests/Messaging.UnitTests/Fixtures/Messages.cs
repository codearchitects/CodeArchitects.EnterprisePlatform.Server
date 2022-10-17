namespace CodeArchitects.Platform.Messaging.Fixtures;

[Message(MessageName)]
public class Message1
{
  public const string MessageName = "msg1";
}

[Message]
public class Message2
{
}

[Message]
public class Message3
{
}

[Message(MessageName)]
public class Message4 : Message1
{
  public new const string MessageName = "msg4";
}

[Message]
public class Message5 : Message3
{

}