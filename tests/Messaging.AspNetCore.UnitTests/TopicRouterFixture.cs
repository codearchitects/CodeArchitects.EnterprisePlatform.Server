using System.Reflection;
using Xunit.Sdk;

namespace CodeArchitects.Platform.Messaging.AspNetCore;

public static class TopicRouterFixture
{
  public class RequestBodyDataAttribute : DataAttribute
  {
    private readonly string _body;
    private readonly string _contentType;
    private readonly int _statusCode;

    public RequestBodyDataAttribute(string body, string contentType, int statusCode)
    {
      _body = body;
      _contentType = contentType;
      _statusCode = statusCode;
    }

    public override IEnumerable<object?[]> GetData(MethodInfo testMethod)
    {
      Stream stream = new MemoryStream();
      StreamWriter writer = new StreamWriter(stream);
      writer.AutoFlush = true;
      writer.Write(_body);
      stream.Position = 0;
      yield return new object?[] { stream, _contentType, _statusCode };
    }
  }
}
