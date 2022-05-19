using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

internal interface ILoggerAccessor
{
  ILogger Logger { get; }
}
