namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IStateDescriptor
{
  Type StateType { get; }

  bool IsStateless { get; }

  IReadOnlyList<object?>? DefaultComponents { get; }
}
