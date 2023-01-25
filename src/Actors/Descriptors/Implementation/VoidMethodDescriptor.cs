using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class VoidMethodDescriptor : MethodDescriptor, IVoidMethodDescriptor
{
  public VoidMethodDescriptor(MethodInfo interfaceMethod, MethodInfo implementationMethod, bool isStateless, int cancellationTokenParameterPosition)
    : base(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition)
  {
  }

  public override MethodKind Kind => MethodKind.Void;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitVoidMethod(this);
  }
}
