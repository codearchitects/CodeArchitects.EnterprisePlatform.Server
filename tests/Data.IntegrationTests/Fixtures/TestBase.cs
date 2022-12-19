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

  Task IAsyncLifetime.InitializeAsync()
  {
    _fixture.Setup(_output);
    return Task.CompletedTask;
  }

  Task IAsyncLifetime.DisposeAsync()
  {
    _fixture.Reset();
    return Task.CompletedTask;
  }
}
