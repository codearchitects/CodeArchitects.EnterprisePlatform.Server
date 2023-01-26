using CodeArchitects.Platform.Actors.Metadata;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class ActorIdDescriptor : IActorIdDescriptor
{
  public abstract Type IdType { get; }

  public abstract bool HasIdSource { get; }
  
  public abstract IStateDependencyDescriptor? StateDependency { get; }
  
  public abstract PropertyInfo? StateProperty { get; }


  public static ActorIdDescriptor Create(IActorMetadata actorMetadata, IImplementationDescriptor baseImplementation)
  {
    ActorIdDescriptor? id = null;
    int index = 0;

    IReadOnlyList<IStateDependencyDescriptor> stateDependencies = baseImplementation.Constructor.StateDependencies;
    foreach (IStateFieldMetadata stateField in actorMetadata.StateFields)
    {
      if (!stateField.IsActorIdSource(out PropertyInfo? actorIdProperty))
        continue;

      if (id is not null)
        throw InvalidActorException.AmbiguousActorIdSource(actorMetadata.ActorType);

      id = new SourceActorIdDescriptor(stateDependencies[index], actorIdProperty);

      index++;
    }

    return id ?? DefaultActorIdDescriptor.Instance;
  }
}
