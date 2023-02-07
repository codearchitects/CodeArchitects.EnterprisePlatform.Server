using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IMethodDescriptor
{
  int Id { get; }

  MethodKind Kind { get; }

  MethodInfo ImplementationMethod { get; }

  Type ReturnType { get; }

  IReadOnlyList<Type> ParameterTypes { get; }

  Type ActivityType { get; }

  IReadOnlyList<FieldInfo> ActivityFields { get; }

  bool HasCancellationTokenParameter { get; }

  int CancellationTokenParameterPosition { get; }

  void Accept(IMethodDescriptorVisitor visitor);
}
