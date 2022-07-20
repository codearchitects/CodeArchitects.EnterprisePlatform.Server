using CodeArchitects.Platform.Messaging.Bindings;

namespace Subscriber.WithResult;

public interface ITestOutputMetadata : IOutputMetadata
{
  string Data { get; }
}

[AttributeUsage(AttributeTargets.ReturnValue)]
public class TestOutputBindingAttribute : Attribute, ITestOutputMetadata
{
  public TestOutputBindingAttribute(string data)
  {
    Data = data;
  }

  public string Data { get; }
}

public class TestOutputBinding : IOutputBinding<ITestOutputMetadata>
{
  private readonly MessageAwaiter _awaiter;

  public TestOutputBinding(MessageAwaiter awaiter)
  {
    _awaiter = awaiter;
  }

  public Task ExecuteAsync<TMessage, TResult>(OutputBindingContext<ITestOutputMetadata, TMessage, TResult> context, CancellationToken cancellationToken)
    where TMessage : class
    where TResult : class
  {
    Result result = (Result)(object)context.Result!;
    _awaiter.Complete(result.Id);
    return Task.CompletedTask;
  }
}
