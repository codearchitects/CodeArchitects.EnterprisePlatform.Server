using CodeArchitects.Platform.Messaging.Bindings;

namespace Subscriber.WithResult;

public interface ITestOutputMetadata : IOutputMetadata
{
  string Data { get; }
  Type? Type { get; }
}

[AttributeUsage(AttributeTargets.ReturnValue)]
public class TestOutputBindingAttribute : Attribute, ITestOutputMetadata
{
  public TestOutputBindingAttribute(string data, Type? type)
  {
    Data = data;
    Type = type;
  }

  public string Data { get; }

  public Type? Type { get; }
}

public class TestOutputBinding : IOutputBinding<ITestOutputMetadata>
{
  private readonly MessageAwaiter _awaiter;

  public TestOutputBinding(MessageAwaiter awaiter)
  {
    _awaiter = awaiter;
  }

  public Task ExecuteAsync<TMessage, TResult>(OutputBindingContext<ITestOutputMetadata, TMessage, TResult> context, CancellationToken cancellationToken)
  {
    Result result = (Result)(object)context.Result!;
    _awaiter.Complete(result.Id);
    return Task.CompletedTask;
  }
}
