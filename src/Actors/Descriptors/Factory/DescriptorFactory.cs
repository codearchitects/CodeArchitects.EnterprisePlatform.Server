using CodeArchitects.Platform.Actors.Descriptors.Implementation;
using CodeArchitects.Platform.Actors.Metadata;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal class DescriptorFactory
{
  private readonly IStateTypeBuilder _stateTypeBuilder;

  public DescriptorFactory(IStateTypeBuilder stateTypeBuilder)
  {
    _stateTypeBuilder = stateTypeBuilder;
  }

  public IActorDescriptor Create(IActorMetadata actorMetadata)
  {
    bool isPolymorphic = actorMetadata.Implementations.Count > 0;
    IStateDescriptor state = CreateState(actorMetadata, isPolymorphic);

    return ActorDescriptor.Create(actorMetadata, state, isPolymorphic);
  }

  private IStateDescriptor CreateState(IActorMetadata actorMetadata, bool isPolymorphic)
  {
    Type actorType = actorMetadata.ActorType;
    if (actorType.IsGenericType)
      throw InvalidActorException.GenericActorsAreNotSupported(actorType);

    IReadOnlyCollection<IStateFieldMetadata> stateFields = actorMetadata.StateFields;
    if (stateFields.Count == 0 && !isPolymorphic)
      return NoStateDescriptor.Instance;

    Type stateType = _stateTypeBuilder.Build(actorType, stateFields.Select(metadata => metadata.Field), isPolymorphic);

    if (actorMetadata.IsExplicitVirtual)
      return CreateVirtual();

    return CreateOrdinary();


    IStateDescriptor CreateVirtual()
    {
      VirtualStateDescriptor descriptor = new(actorType, stateType);
      foreach (IStateFieldMetadata metadata in stateFields)
      {
        descriptor.AddField(metadata.Field, metadata.DefaultValue);
      }

      return descriptor;
    }

    IStateDescriptor CreateOrdinary()
    {
      OrdinaryStateDescriptor descriptor = new(actorType, stateType);
      foreach (IStateFieldMetadata metadata in stateFields)
      {
        descriptor.AddField(metadata.Field);
      }

      return descriptor;
    }
  }
}
