using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class VoidMethodDescriptor : MethodDescriptor, IVoidMethodDescriptor
{
  public VoidMethodDescriptor(int id, MethodInfo implementationMethod, Type[] parameterTypes, Func<IMethodDescriptor, Type> activityTypeFactory)
    : base(id, null, implementationMethod, parameterTypes, activityTypeFactory)
  {
  }

  public override MethodKind Kind => MethodKind.Void;

  public override void Accept(IMethodDescriptorVisitor visitor)
  {
    visitor.VisitVoidMethod(this);
  }
}
