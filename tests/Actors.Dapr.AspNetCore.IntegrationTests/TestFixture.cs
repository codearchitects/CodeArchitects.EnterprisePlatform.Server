using CliWrap;
using CliWrap.Exceptions;
using System.Net;
using System.Text;

namespace CodeArchitects.Platform.Actors.Dapr.AspNetCore;

public class TestFixture : IAsyncLifetime
{
  private static readonly string s_dockerComposePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../.."));
  private static readonly Uri s_daprHealthUri = new("http://localhost:3500/v1.0/healthz");
  private static readonly Uri s_appHealthUri = new("http://localhost:20100/virtual-actor/binding-disabler");
  private readonly IReadOnlyDictionary<string, string?> _envVars;
  private const string LATEST_FRAMEWORK = "10.0";

  public TestFixture()
  {
    string targetFrameworkMoniker;
    string httpPort;

#if NET10_0
    targetFrameworkMoniker = "10.0";
    httpPort = "8080";
#elif NET9_0
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

    await WaitForServiceReadinessAsync();
  }

  public async Task DisposeAsync()
  {
    await Cli.Wrap("docker")
      .WithArguments("compose down")
      .WithWorkingDirectory(s_dockerComposePath)
      .WithEnvironmentVariables(_envVars)
      .ExecuteAsync();
  }

  private async Task WaitForServiceReadinessAsync()
  {
    using HttpClient http = new();
    using CancellationTokenSource cts = new(TimeSpan.FromSeconds(90));

    Exception? daprLastError = null;
    Exception? appLastError = null;

    while (!cts.Token.IsCancellationRequested)
    {
      try
      {
        using HttpResponseMessage response = await http.GetAsync(s_daprHealthUri, cts.Token);
        if (response.IsSuccessStatusCode)
        {
          daprLastError = null;
          break;
        }

        daprLastError = new InvalidOperationException($"Unexpected Dapr health status code {(int)response.StatusCode}.");
      }
      catch (Exception ex)
      {
        daprLastError = ex;
      }

      await Task.Delay(1000);
    }

    if (daprLastError is not null)
    {
      throw new TimeoutException($"Dapr sidecar did not become ready at {s_daprHealthUri}.", daprLastError);
    }

    while (!cts.Token.IsCancellationRequested)
    {
      try
      {
        using HttpResponseMessage response = await http.GetAsync(s_appHealthUri, cts.Token);
        if (response.StatusCode == HttpStatusCode.OK)
        {
          return;
        }

        appLastError = new InvalidOperationException($"Unexpected app readiness status code {(int)response.StatusCode}.");
      }
      catch (Exception ex)
      {
        appLastError = ex;
      }

      await Task.Delay(1000);
    }

    throw new TimeoutException($"Actor app did not become ready at {s_appHealthUri}.", appLastError);
  }
}
