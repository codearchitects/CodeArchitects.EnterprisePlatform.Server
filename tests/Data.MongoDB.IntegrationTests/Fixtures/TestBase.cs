namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

public class TestBase : IAsyncLifetime
{
  protected readonly TestFixture _fixture;

  public TestBase(TestFixture fixture)
  {
    _fixture = fixture;
  }

  public Task InitializeAsync()
  {
    return Task.CompletedTask;
  }

  public async Task DisposeAsync()
  {
    await _fixture.ResetAsync();
  }
}
