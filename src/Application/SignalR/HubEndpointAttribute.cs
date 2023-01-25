namespace CodeArchitects.Platform.Application.SignalR;

/// <summary>
/// Specifies the hub endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HubEndpointAttribute : Attribute
{
  public HubEndpointAttribute(string endpoint)
  {
    Endpoint = endpoint;
  }

  /// <summary>
  /// The hub endpoint.
  /// </summary>
  public string Endpoint { get; }
}
