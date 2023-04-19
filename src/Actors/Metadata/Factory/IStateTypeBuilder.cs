namespace CodeArchitects.Platform.Actors.Metadata.Factory;

internal interface IStateTypeBuilder
{
  Type Build(Type actorType, IEnumerable<IStateComponentMetadata> components, bool isPolymorphic);
}
