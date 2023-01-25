namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IImplementationDescriptor
{
  Type Type { get; }

  IConstructorDescriptor Constructor { get; }

  IReadOnlyList<IMethodDescriptor> Methods { get; }
}
