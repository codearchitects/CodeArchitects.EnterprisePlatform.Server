using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class TaskTMethodDescriptor : MethodDescriptor, ITaskTMethodDescriptor
{
  private Type? _resultType;

  public TaskTMethodDescriptor(MethodInfo interfaceMethod, MethodInfo implementationMethod, bool isStateless, int cancellationTokenParameterPosition)
    : base(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition)
  {
  }

  public Type ResultType => _resultType ??= InterfaceMethod.ReturnType.GetGenericArguments()[0];

  public override MethodKind Kind => MethodKind.TaskT;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitTaskTMethod(this);
  }
}
