using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IMethodDescriptor
{
  int Id { get; }

  MethodKind Kind { get; }

  string Name { get; }

  MethodInfo? InterfaceMethod { get; }

  MethodInfo ImplementationMethod { get; }

  Type ReturnType { get; }

  Type[] ParameterTypes { get; }

  Type ActivityType { get; }

  IReadOnlyList<FieldInfo> ActivityFields { get; }

  bool HasCancellationTokenParameter { get; }

  void Accept(IMethodDescriptorVisitor visitor);
}
