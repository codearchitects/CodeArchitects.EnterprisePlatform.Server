namespace CodeArchitects.Platform.Messaging.AspNetCore.Fixtures;

public class Message1
{
  public string? Data { get; set; }
}

public class Message2
{
  public string? Data { get; set; }
}

public class Message3 : Message1
{
}

public static class JsonMessages
{
  public const string GoodMessageRawPayload =
"""
{
  "$type": "MyMessage",
  "content": "myContent"
}
""";

  public const string GoodMessageCloudEvent =
"""
{
  "specversion": "1.0",
  "type": "MyMessage",
  "source": "source",
  "id": "9aeb0fdf-c01e-0131-0922-9eb54906e209",
  "time": "2019-11-18T15:13:39.4589254Z",
  "subject": "subject",
  "data": {
    "content": "myContent"
  }
}
""";

  public const string BadMessageRawPayload1 =
"""
{
  "content": "myContent"
}
""";

  public const string BadMessageRawPayload2 =
"""
{
  "$type": 2,
  "content": "myContent"
}
""";

  public const string BadMessageCloudEvent1 =
"""
{
  "data": {
    "content": "myContent"
  }
}
""";

  public const string BadMessageCloudEvent2 =
"""
{
  "type": "MyMessage",
  "data": "data"
}
""";

  public const string BadMessageCloudEvent3 =
"""
{
  "type": "MyMessage"
}
""";

  public const string BadMessageCloudEvent4 =
"""
{
  "type": "MyMessage",
  "data": "data",
  "data_base64": "data_base64"
}
""";

  public const string UnsupportedMessageCloudEvent =
"""
{
  "type": "MyMessage",
  "data_base64": "data_base64"
}
""";
}