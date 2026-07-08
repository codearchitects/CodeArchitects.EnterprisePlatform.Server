using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.Fixtures;

public class TestBase : IAsyncLifetime
{
  protected readonly TestFixture _fixture;
  protected readonly ITestOutputHelper _output;

  public TestBase(TestFixture fixture, ITestOutputHelper output)
  {
    _fixture = fixture;
    _output = output;
  }

  protected virtual Task InitializeAsync()
  {
    _fixture.Setup(_output);
    return Task.CompletedTask;
  }

  protected virtual Task DisposeAsync()
  {
    _fixture.Reset();
    return Task.CompletedTask;
  }

  Task IAsyncLifetime.InitializeAsync() => InitializeAsync();

  Task IAsyncLifetime.DisposeAsync() => DisposeAsync();
}
