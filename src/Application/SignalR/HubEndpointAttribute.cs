namespace CodeArchitects.Platform.Application.SignalR;

/// <summary>
/// Specifies the hub endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HubEndpointAttribute : Attribute
{
  /// <summary>
  /// Creates a new <see cref="HubEndpointAttribute"/>.
  /// </summary>
  /// <param name="endpoint">The hub endpoint.</param>
  public HubEndpointAttribute(string endpoint)
  {
    Endpoint = endpoint;
  }

  /// <summary>
  /// The hub endpoint.
  /// </summary>
  public string Endpoint { get; }
}
