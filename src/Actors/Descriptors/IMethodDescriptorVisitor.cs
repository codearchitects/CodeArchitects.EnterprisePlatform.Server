namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IMethodDescriptorVisitor
{
  void VisitTaskMethod(ITaskMethodDescriptor descriptor);

  void VisitTaskTMethod(ITaskTMethodDescriptor descriptor);
  
  void VisitValueTaskMethod(IValueTaskMethodDescriptor descriptor);
  
  void VisitValueTaskTMethod(IValueTaskTMethodDescriptor descriptor);
}
