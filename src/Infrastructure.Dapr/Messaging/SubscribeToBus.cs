using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using System;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  /// <summary>
  /// Indicates that a handler is subscribed to a given bus.
  /// </summary>
  /// <remarks>
  /// If not applied, the default bus name will be resolved from the <see cref="DaprConfiguration"/> instance in the application services.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public class SubscribeToBusAttribute : Attribute
  {
    /// <summary>
    /// Creates a new <see cref="SubscribeToBusAttribute"/> instance.
    /// </summary>
    /// <param name="busName">The name of the bus.</param>
    public SubscribeToBusAttribute(string busName)
    {
      if (string.IsNullOrWhiteSpace(busName)) throw new ArgumentException($"'{nameof(busName)}' cannot be null or whitespace.", nameof(busName));

      BusName = busName;
    }

    /// <summary>
    /// The name of the bus.
    /// </summary>
    public string BusName { get; }
  }
}
