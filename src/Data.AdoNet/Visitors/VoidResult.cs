namespace CodeArchitects.Platform.Data.AdoNet.Visitors;

/// <summary>
/// Type that can be used instead of <see cref="void"/> as a generic argument.
/// </summary>
public class VoidResult
{
  /// <summary>
  /// The <see cref="VoidResult"/> singleton.
  /// </summary>
  public static readonly VoidResult Instance = new();

  private VoidResult() { }
}
