using CodeArchitects.Platform.Actors.Descriptors;

namespace CodeArchitects.Platform.Actors.Scheduling;
internal interface IActivityTypeBuilder
{
  Type Build(IMethodDescriptor descriptor, Type actorType, Type baseType);
  Type BuildBase(Type actorType);
}