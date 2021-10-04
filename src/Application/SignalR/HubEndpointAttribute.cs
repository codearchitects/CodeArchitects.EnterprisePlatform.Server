using System;

namespace CodeArchitects.Platform.Application.SignalR
{
  [AttributeUsage(AttributeTargets.Class)]
  public class HubEndpointAttribute : Attribute
  {
    public HubEndpointAttribute(string pattern)
    {
      Pattern = pattern;
    }

    public string Pattern { get; }
  }
}
