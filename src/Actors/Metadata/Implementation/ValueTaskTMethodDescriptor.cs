using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal class ValueTaskTMethodDescriptor : MethodDescriptor, IValueTaskTMethodDescriptor
{
  public ValueTaskTMethodDescriptor(int id, MethodInfo? interfaceMethod, MethodInfo implementationMethod, Type[] parameterTypes, Func<IMethodDescriptor, Type> activityTypeFactory)
    : base(id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory)
  {
  }

  public override MethodKind Kind => MethodKind.ValueTaskT;

  public Type ResultType => ReturnType.GetGenericArguments()[0];

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitValueTaskTMethod(this);
  }
}
