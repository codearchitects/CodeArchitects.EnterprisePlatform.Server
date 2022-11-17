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
    return _fixture.SetUpAsync(_output);
  }

  async Task IAsyncLifetime.DisposeAsync()
  {
    await _fixture.ResetAsync();
  }
}
