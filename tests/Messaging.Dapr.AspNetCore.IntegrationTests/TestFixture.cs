using CliWrap;
using CliWrap.Exceptions;
using System.Text;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

public class TestFixture : IAsyncLifetime
{
  private static readonly string s_dockerComposePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../.."));

  public async Task InitializeAsync()
  {
    StringBuilder errorSb = new();

    try
    {
      await Cli.Wrap("docker-compose")
        .WithArguments("build --no-cache")
        .WithWorkingDirectory(s_dockerComposePath)
        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(errorSb))
        .ExecuteAsync();

      await Cli.Wrap("docker-compose")
        .WithArguments("up -d")
        .WithWorkingDirectory(s_dockerComposePath)
        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(errorSb))
        .ExecuteAsync();
    }
    catch (CommandExecutionException ex)
    {
      throw new Exception($"Exception in executing the command. Error log:\n{errorSb}", ex);
    }

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
