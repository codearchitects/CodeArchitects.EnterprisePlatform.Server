namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface ITaskTMethodDescriptor : IMethodDescriptor
{
  Type ResultType { get; }
}
