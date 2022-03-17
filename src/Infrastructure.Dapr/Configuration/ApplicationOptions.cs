using System;
using System.Collections.Generic;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Configuration;

public class ApplicationOptions
{
  public IReadOnlyList<string> MessageBusses { get; set; } = Array.Empty<string>();
  public IReadOnlyList<string> StateStores { get; set; } = Array.Empty<string>();
}
