namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IImplementationDescriptor
{
  int Id { get; }

  Type Type { get; }
}
