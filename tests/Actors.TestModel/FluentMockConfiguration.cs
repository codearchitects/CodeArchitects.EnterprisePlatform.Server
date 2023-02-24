using CodeArchitects.Platform.Actors.Descriptors;
using FluentMock;

[assembly: GenerateFluentMockFor(typeof(IActorDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IImplementationDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IActorIdDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IStateDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IActorFactoryDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IMethodDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IVoidMethodDescriptor))]
[assembly: GenerateFluentMockFor(typeof(ITaskMethodDescriptor))]
[assembly: GenerateFluentMockFor(typeof(ITaskTMethodDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IValueTaskMethodDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IValueTaskTMethodDescriptor))]
[assembly: GenerateFluentMockFor(typeof(IMessageHandlerDescriptor))]
