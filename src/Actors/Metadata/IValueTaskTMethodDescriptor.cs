namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IValueTaskTMethodDescriptor : IMethodDescriptor
{
  Type ResultType { get; }
}
