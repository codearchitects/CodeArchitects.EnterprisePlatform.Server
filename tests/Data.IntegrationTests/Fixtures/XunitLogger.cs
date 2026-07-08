using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Data.Fixtures;

public class XunitLogger : ILogger
{
  private readonly TestFixture _fixture;

  public XunitLogger(TestFixture fixture)
  {
    _fixture = fixture;
  }

  public IDisposable BeginScope<TState>(TState state)
  {
    return new Scope();
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return true;
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    _fixture.Output?.WriteLine($"[{logLevel}]: {formatter(state, exception)}");
  }

  private sealed class Scope : IDisposable
  {
    public void Dispose() { }
  }
}

public class XunitLoggerFactory : ILoggerFactory
{
  private readonly TestFixture _fixture;

  public XunitLoggerFactory(TestFixture fixture)
  {
    _fixture = fixture;
  }

  public void AddProvider(ILoggerProvider provider)
  {
  }

  public ILogger CreateLogger(string categoryName)
  {
    return new XunitLogger(_fixture);
  }

  public void Dispose()
  {
  }
}
