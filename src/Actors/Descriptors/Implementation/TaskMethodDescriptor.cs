using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class TaskMethodDescriptor : MethodDescriptor, ITaskMethodDescriptor
{
  public TaskMethodDescriptor(int id, MethodInfo? interfaceMethod, MethodInfo implementationMethod, Type[] parameterTypes, Func<IMethodDescriptor, Type> activityTypeFactory)
    : base(id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory)
  {
  }

  public override MethodKind Kind => MethodKind.Task;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitTaskMethod(this);
  }
}
