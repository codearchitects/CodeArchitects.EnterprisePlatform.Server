namespace CodeArchitects.Platform.Actors.Metadata.Implementation;

internal abstract class ActorMetadata : IActorMetadata
{
  private readonly List<StateFieldMetadata> _stateFields;
  private readonly Dictionary<Type, ImplementationMetadata> _implementations;

  public ActorMetadata()
  {
    _stateFields = new();
    _implementations = new();
  }

  public abstract Type? InterfaceType { get; }

  public abstract bool IsExplicitVirtual { get; }

  public abstract IImplementationMetadata BaseImplementation { get; }

  public Type ActorType => BaseImplementation.ImplementationType;

  public Type? FactoryType { get; protected set; }

  public IReadOnlyList<IStateFieldMetadata> StateFields => _stateFields;

  public IReadOnlyCollection<IImplementationMetadata> Implementations => _implementations.Values;

  protected void AddStateField(StateFieldMetadata stateField)
  {
    _stateFields.Add(stateField);
  }

  protected void AddImplementation(ImplementationMetadata implementation)
  {
    _implementations[implementation.ImplementationType] = implementation;
  }
}
