using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IMethodDescriptor
{
  MethodKind Kind { get; }
  
  MethodInfo InterfaceMethod { get; }
  
  MethodInfo ImplementationMethod { get; }
  
  IReadOnlyList<Type> ParameterTypes { get; }
  
  bool IsStateless { get; }
  
  bool HasCancellationTokenParameter { get; }
  
  int CancellationTokenParameterPosition { get; }

  void Accept(IMethodDescriptorVisitor visitor);
}
