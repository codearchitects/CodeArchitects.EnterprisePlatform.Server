using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Data.Fixtures;

public class XunitLogger : ILogger
{
  private readonly TestLocalData _localData;

  public XunitLogger(TestLocalData localData)
  {
    _localData = localData;
  }

  public IDisposable? BeginScope<TState>(TState state)
    where TState : notnull
  {
    return null;
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return true;
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    _localData.Output?.WriteLine($"[{logLevel}]: {formatter(state, exception)}");
  }
}

public class XunitLoggerFactory : ILoggerFactory
{
  private readonly TestLocalData _localData;

  public XunitLoggerFactory(TestLocalData localData)
  {
    _localData = localData;
  }

  public void AddProvider(ILoggerProvider provider)
  {
  }

  public ILogger CreateLogger(string categoryName)
  {
    return new XunitLogger(_localData);
  }

  public void Dispose()
  {
  }
}
