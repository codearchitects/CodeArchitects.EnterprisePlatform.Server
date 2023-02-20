using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.FluentMock;

namespace CodeArchitects.Platform.Actors.TestModel;

internal static class DescriptorBuilderExtensions
{
  public static VoidMethodDescriptorBuilder InitDefaults(this VoidMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.Void)
      .SetReturnType(typeof(void))
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitVoidMethod(mock.Object)));
  }

  public static TaskMethodDescriptorBuilder InitDefaults(this TaskMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.Task)
      .SetReturnType(typeof(Task))
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitTaskMethod(mock.Object)));
  }

  public static TaskTMethodDescriptorBuilder InitDefaults(this TaskTMethodDescriptorBuilder builder, Type resultType)
  {
    return builder
      .SetKind(MethodKind.TaskT)
      .SetReturnType(typeof(Task<>).MakeGenericType(resultType))
      .SetResultType(resultType)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitTaskTMethod(mock.Object)));
  }

  public static ValueTaskMethodDescriptorBuilder InitDefaults(this ValueTaskMethodDescriptorBuilder builder)
  {
    return builder
      .SetKind(MethodKind.ValueTask)
      .SetReturnType(typeof(ValueTask))
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitValueTaskMethod(mock.Object)));
  }

  public static ValueTaskTMethodDescriptorBuilder InitDefaults(this ValueTaskTMethodDescriptorBuilder builder, Type resultType)
  {
    return builder
      .SetKind(MethodKind.ValueTaskT)
      .SetReturnType(typeof(ValueTask<>).MakeGenericType(resultType))
      .SetResultType(resultType)
      .Setup(mock => mock
        .Setup(x => x.Accept(It.IsAny<IMethodDescriptorVisitor>()))
        .Callback<IMethodDescriptorVisitor>(visitor => visitor.VisitValueTaskTMethod(mock.Object)));
  }
}
