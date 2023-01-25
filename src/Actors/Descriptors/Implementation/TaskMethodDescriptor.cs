using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class TaskMethodDescriptor : MethodDescriptor, ITaskMethodDescriptor
{
  public TaskMethodDescriptor(MethodInfo interfaceMethod, MethodInfo implementationMethod, bool isStateless, int cancellationTokenParameterPosition)
    : base(interfaceMethod, implementationMethod, isStateless, cancellationTokenParameterPosition)
  {
  }

  public override MethodKind Kind => MethodKind.Task;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitTaskMethod(this);
  }
}
