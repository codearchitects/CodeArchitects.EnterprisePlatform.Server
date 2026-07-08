namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IImplementationDescriptor
{
  int Id { get; }

  Type Type { get; }
}
