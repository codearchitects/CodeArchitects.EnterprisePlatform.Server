using CodeArchitects.Platform.Actors.Metadata;

namespace CodeArchitects.Platform.Actors.Scheduling;
internal interface IActivityTypeBuilder
{
  Type Build(IMethodDescriptor descriptor, Type actorType, Type baseType);
  Type BuildBase(Type actorType);
}