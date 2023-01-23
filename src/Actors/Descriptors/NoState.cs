namespace CodeArchitects.Platform.Actors.Descriptors;

internal class NoState
{
  public static readonly NoState Instance = new NoState();

  private NoState() { }
}
