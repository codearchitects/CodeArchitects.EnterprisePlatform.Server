using CodeArchitects.Platform.Actors.Metadata;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal abstract class ActorIdDescriptor : IActorIdDescriptor
{
  public abstract Type IdType { get; }

  public abstract bool HasIdSource { get; }
  
  public abstract IStateDependencyDescriptor? StateDependency { get; }
  
  public abstract PropertyInfo? StateProperty { get; }


  public static ActorIdDescriptor Create(IActorMetadata actorMetadata, IReadOnlyList<IStateDependencyDescriptor> stateDependencies)
  {
    ActorIdDescriptor? id = null;
    int index = 0;

    foreach (IStateFieldMetadata stateField in actorMetadata.StateFields)
    {
      if (!stateField.IsActorIdSource)
        continue;

      if (id is not null)
        throw InvalidActorException.AmbiguousActorIdSource(actorMetadata.ActorType);

      id = new SourceActorIdDescriptor(stateDependencies[index], stateField.ActorIdProperty);

      index++;
    }

    return id ?? DefaultActorIdDescriptor.Instance;
  }
}
