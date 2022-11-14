using CliWrap;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

public class TestFixture : IAsyncLifetime
{
  private static readonly string s_dockerComposePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../.."));

  public async Task InitializeAsync()
  {
    await Cli.Wrap("docker")
      .WithArguments("compose up --detach")
      .WithWorkingDirectory(s_dockerComposePath)
      .ExecuteAsync();
    await Task.Delay(10000);
  }

  public async Task DisposeAsync()
  {
    await Cli.Wrap("docker")
      .WithArguments("compose down")
      .WithWorkingDirectory(s_dockerComposePath)
      .ExecuteAsync();
  }
}
