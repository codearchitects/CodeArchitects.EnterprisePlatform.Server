using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ValueTaskMethodDescriptor : MethodDescriptor, IValueTaskMethodDescriptor
{
  public ValueTaskMethodDescriptor(MethodInfo interfaceMethod, MethodInfo implementationMethod, bool isStateless, int cancellationTokenParameterPosition)
    : base(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition)
  {
  }

  public override MethodKind Kind => MethodKind.ValueTask;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitValueTaskMethod(this);
  }
}
