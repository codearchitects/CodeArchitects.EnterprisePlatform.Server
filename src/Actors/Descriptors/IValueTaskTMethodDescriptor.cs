namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IValueTaskTMethodDescriptor : IMethodDescriptor
{
  Type ResultType { get; }
}
