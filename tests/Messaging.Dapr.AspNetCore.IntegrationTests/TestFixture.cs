using CliWrap;
using CliWrap.Exceptions;
using System.Net;
using System.Text;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore;

public class TestFixture : IAsyncLifetime
{
  private static readonly string s_dockerComposePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../.."));
  private static readonly Uri s_subscriberWaitBaseUri = new("http://localhost:20001/wait/");
  private static readonly Uri s_publisherSendBaseUri = new("http://localhost:20000/noresult/by-reflection/send/");
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

  private static async Task WaitForServiceReadinessAsync()
  {
    using HttpClient http = new();
    using CancellationTokenSource cts = new(TimeSpan.FromSeconds(120));

    Exception? lastError = null;

    while (!cts.Token.IsCancellationRequested)
    {
      Guid probeId = Guid.NewGuid();

      try
      {
        // Subscriber readiness (process up and endpoint mapped)
        using HttpResponseMessage waitProbe = await http.GetAsync(new Uri(s_subscriberWaitBaseUri, $"{probeId}?millisecondsTimeout=1"), cts.Token);
        if (waitProbe.StatusCode != HttpStatusCode.OK && waitProbe.StatusCode != HttpStatusCode.NoContent)
        {
          throw new InvalidOperationException($"Unexpected subscriber status code {(int)waitProbe.StatusCode}.");
        }

        // Publisher + sidecar + bus readiness (fails with 500 while Dapr/pubsub not ready)
        using HttpResponseMessage sendProbe = await http.GetAsync(new Uri(s_publisherSendBaseUri, probeId.ToString()), cts.Token);
        if (sendProbe.StatusCode == HttpStatusCode.OK)
        {
          return;
        }

        string body = await sendProbe.Content.ReadAsStringAsync(cts.Token);
        throw new InvalidOperationException($"Unexpected publisher status code {(int)sendProbe.StatusCode}. Body: {body}");
      }
      catch (Exception ex)
      {
        lastError = ex;
      }

      await Task.Delay(1000);
    }

    throw new TimeoutException("Messaging integration services did not become ready in time.", lastError);
  }
}
