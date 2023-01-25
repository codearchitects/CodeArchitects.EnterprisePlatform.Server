using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ValueTaskTMethodDescriptor : MethodDescriptor, IValueTaskTMethodDescriptor
{
  private Type? _resultType;

  public ValueTaskTMethodDescriptor(MethodInfo interfaceMethod, MethodInfo implementationMethod, bool isStateless, int cancellationTokenParameterPosition)
    : base(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition)
  {
  }

  public Type ResultType => _resultType ??= InterfaceMethod.ReturnType.GetGenericArguments()[0];

  public override MethodKind Kind => MethodKind.ValueTaskT;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitValueTaskTMethod(this);
  }
}
