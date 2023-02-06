using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IMethodDescriptor : IActivityDescriptor
{
  MethodKind Kind { get; }
  
  MethodInfo InterfaceMethod { get; }

  void Accept(IMethodDescriptorVisitor visitor);
}
