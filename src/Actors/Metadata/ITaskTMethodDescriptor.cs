namespace CodeArchitects.Platform.Actors.Metadata;

internal interface ITaskTMethodDescriptor : IMethodDescriptor
{
  Type ResultType { get; }
}
