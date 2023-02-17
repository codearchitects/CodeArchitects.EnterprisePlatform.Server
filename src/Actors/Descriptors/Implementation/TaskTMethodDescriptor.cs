using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class TaskTMethodDescriptor : MethodDescriptor, ITaskTMethodDescriptor
{
  public TaskTMethodDescriptor(int id, MethodInfo? interfaceMethod, MethodInfo implementationMethod, Type[] parameterTypes, Func<IMethodDescriptor, Type> activityTypeFactory)
    : base(id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory)
  {
  }

  public override MethodKind Kind => MethodKind.TaskT;

  public Type ResultType => ReturnType.GetGenericArguments()[0];

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitTaskTMethod(this);
  }
}
