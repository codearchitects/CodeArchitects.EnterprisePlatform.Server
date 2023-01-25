using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;

namespace CodeArchitects.Platform.Actors.Fixtures;

internal static class DescriptorBuilderExtensions
{
  public static ContextDependencyDescriptorBuilder InitDefaults(this ContextDependencyDescriptorBuilder builder)
  {
    return builder
      .SetKind(DependencyKind.Context)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IDependencyDescriptorVisitor>()))
        .Callback<IDependencyDescriptorVisitor>(visitor => visitor.VisitContextDependency(mock.Object)));
  }

  public static ServiceDependencyDescriptorBuilder InitDefaults(this ServiceDependencyDescriptorBuilder builder)
  {
    return builder
      .SetKind(DependencyKind.Service)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IDependencyDescriptorVisitor>()))
        .Callback<IDependencyDescriptorVisitor>(visitor => visitor.VisitServiceDependency(mock.Object)));
  }

  public static StateDependencyDescriptorBuilder InitDefaults(this StateDependencyDescriptorBuilder builder)
  {
    return builder
      .SetKind(DependencyKind.State)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IDependencyDescriptorVisitor>()))
        .Callback<IDependencyDescriptorVisitor>(visitor => visitor.VisitStateDependency(mock.Object)));
  }

  public static VoidMethodDescriptorBuilder InitDefaults(this VoidMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.Void)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitVoidMethod(mock.Object)));
  }

  public static TaskMethodDescriptorBuilder InitDefaults(this TaskMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.Task)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitTaskMethod(mock.Object)));
  }

  public static TaskTMethodDescriptorBuilder InitDefaults(this TaskTMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.TaskT)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitTaskTMethod(mock.Object)));
  }

  public static ValueTaskMethodDescriptorBuilder InitDefaults(this ValueTaskMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.ValueTask)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitValueTaskMethod(mock.Object)));
  }

  public static ValueTaskTMethodDescriptorBuilder InitDefaults(this ValueTaskTMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.ValueTaskT)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitValueTaskTMethod(mock.Object)));
  }
}
