namespace CodeArchitects.Platform.Actors.Scheduling;

internal abstract class Activity
{
  public abstract int Id { get; }

  public int ImplementationId { get; set; }
}
