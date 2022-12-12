namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

public class VoidResult
{
  public static readonly VoidResult Instance = new VoidResult();

  private VoidResult() { }
}
