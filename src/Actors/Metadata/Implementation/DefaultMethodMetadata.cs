namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class DefaultMethodMetadata : IMethodMetadata
{
  public static readonly DefaultMethodMetadata Instance = new();

  private DefaultMethodMetadata() { }

  public bool IsStateless => false;
}
