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
    IStateDescriptor state = CreateState(actorMetadata);

    return ActorDescriptor.Create(actorMetadata, state);
  }

  private IStateDescriptor CreateState(IActorMetadata actorMetadata)
  {
    Type actorType = actorMetadata.ActorType;
    IReadOnlyCollection<IStateFieldMetadata> stateFields = actorMetadata.StateFields;

    if (stateFields.Count == 0)
      return NoStateDescriptor.Instance;

    Type stateType = _stateTypeBuilder.Build(actorType, stateFields.Select(metadata => metadata.Field));

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
