using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ValueTaskMethodDescriptor : MethodDescriptor, IValueTaskMethodDescriptor
{
  public ValueTaskMethodDescriptor(int id, MethodInfo? interfaceMethod, MethodInfo implementationMethod, Type[] parameterTypes, Func<IMethodDescriptor, Type> activityTypeFactory)
    : base(id, interfaceMethod, implementationMethod, parameterTypes, activityTypeFactory)
  {
  }

  public override MethodKind Kind => MethodKind.ValueTask;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitValueTaskMethod(this);
  }
}
