namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal interface IStateTypeBuilder
{
  Type Build(Type actorType, IEnumerable<IStateComponentMetadata> components, bool isPolymorphic);
}
