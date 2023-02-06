using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActivityDescriptor
{
  int Id { get; }

  MethodInfo ImplementationMethod { get; }

  IReadOnlyList<Type> ParameterTypes { get; }

  Type PayloadType { get; }

  IReadOnlyList<FieldInfo> PayloadFields { get; }

  bool HasCancellationTokenParameter { get; }

  int CancellationTokenParameterPosition { get; }
}
