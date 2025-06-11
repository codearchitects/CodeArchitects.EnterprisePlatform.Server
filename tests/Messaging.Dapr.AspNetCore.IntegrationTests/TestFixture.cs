using CliWrap;
using CliWrap.Exceptions;
using System.Text;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

public class TestFixture : IAsyncLifetime
{
  private static readonly string s_dockerComposePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../.."));
  private readonly IReadOnlyDictionary<string, string?> _envVars;
  private const string LATEST_FRAMEWORK = "9.0";

  public TestFixture()
  {
    string targetFrameworkMoniker;
    string httpPort;

#if NET9_0
    targetFrameworkMoniker = "9.0";
    httpPort = "8080";
#elif NET8_0
    targetFrameworkMoniker = "8.0";
    httpPort = "8080";
#elif NET7_0
    targetFrameworkMoniker = "7.0";
    httpPort = "80";
#endif

    _envVars = new Dictionary<string, string?>()
    {
      { "TARGET_FRAMEWORK", targetFrameworkMoniker },
      { nameof(LATEST_FRAMEWORK), LATEST_FRAMEWORK },
      { "HTTP_PORT", httpPort }
    };
  }

  public async Task InitializeAsync()
  {
    StringBuilder errorSb = new();

    try
    {
      await Cli.Wrap("docker-compose")
        .WithArguments("build --no-cache")
        .WithWorkingDirectory(s_dockerComposePath)
        .WithEnvironmentVariables(_envVars)
        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(errorSb))
        .ExecuteAsync();

      await Cli.Wrap("docker-compose")
        .WithArguments("up -d")
        .WithWorkingDirectory(s_dockerComposePath)
        .WithEnvironmentVariables(_envVars)
        .WithStandardErrorPipe(PipeTarget.ToStringBuilder(errorSb))
        .ExecuteAsync();
    }
    catch (CommandExecutionException ex)
    {
      throw new Exception($"Exception in executing the command. Error log:\n{errorSb}", ex);
    }

    await Task.Delay(15000);
  }

  public async Task DisposeAsync()
  {
    await Cli.Wrap("docker")
      .WithArguments("compose down")
      .WithWorkingDirectory(s_dockerComposePath)
      .WithEnvironmentVariables(_envVars)
      .ExecuteAsync();
  }
}
