namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IMethodDescriptorVisitor
{
  void VisitVoidMethod(IVoidMethodDescriptor descriptor);
  
  void VisitTaskMethod(ITaskMethodDescriptor descriptor);
  
  void VisitTaskTMethod(ITaskTMethodDescriptor descriptor);
  
  void VisitValueTaskMethod(IValueTaskMethodDescriptor descriptor);
  
  void VisitValueTaskTMethod(IValueTaskTMethodDescriptor descriptor);
}
