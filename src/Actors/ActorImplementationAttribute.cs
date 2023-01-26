namespace CodeArchitects.Platform.Actors;

[AttributeUsage(AttributeTargets.Class)]
public class ActorImplementationAttribute : Attribute
{
  public bool IsDefault { get; set; }
}
