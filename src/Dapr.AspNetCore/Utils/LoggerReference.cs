using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Utils;

[ExcludeFromCodeCoverage]
internal class LoggerReference : ILogger
{
  public LoggerReference(ILogger logger)
  {
    Logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  public ILogger Logger { get; set; }

  IDisposable ILogger.BeginScope<TState>(TState state)
  {
    return Logger.BeginScope(state) ?? throw new InvalidOperationException("Logger.BeginScope returned null.");
  }

  public bool IsEnabled(LogLevel logLevel)
  {
    return Logger.IsEnabled(logLevel);
  }

  public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
  {
    Logger.Log(logLevel, eventId, state, exception, formatter);
  }
}
