using CodeArchitects.Platform.Actors.Metadata;
using System.Diagnostics;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ConstructorDescriptor : IConstructorDescriptor
{
  private readonly DependencyAdder _dependencyAdder;
  private readonly List<IDependencyDescriptor> _dependencies;
  private readonly List<IServiceDependencyDescriptor> _serviceDependencies;
  private readonly List<IStateDependencyDescriptor> _stateDependencies;

  public ConstructorDescriptor(Type actorType, ConstructorInfo constructor)
  {
    Constructor = constructor;
    _dependencyAdder = new DependencyAdder(actorType, this);
    _dependencies = new();
    _serviceDependencies = new();
    _stateDependencies = new();
  }

  public ConstructorInfo Constructor { get; }

  public IReadOnlyList<IDependencyDescriptor> Dependencies => _dependencies;

  public IContextDependencyDescriptor? ContextDependency { get; private set; }

  public IReadOnlyList<IServiceDependencyDescriptor> ServiceDependencies => _serviceDependencies;

  public IReadOnlyList<IStateDependencyDescriptor> StateDependencies => _stateDependencies;

  public void AddDependency(IDependencyDescriptor dependency)
  {
    _dependencyAdder.Add(dependency);
  }


  public static ConstructorDescriptor Create(IActorMetadata actorMetadata, ConstructorInfo constructor)
  {
    Type actorType = actorMetadata.ActorType;
    ConstructorDescriptor descriptor = new(actorType, constructor);
    foreach (DependencyDescriptor dependency in DependencyDescriptor.CreateMany(actorMetadata, constructor))
    {
      descriptor.AddDependency(dependency);
    }

    if (descriptor.StateDependencies.Count < actorMetadata.StateFields.Count)
      throw InvalidActorException.StateComponentsMismatch(actorType);

    Debug.Assert(descriptor.StateDependencies.Count == actorMetadata.StateFields.Count, "Found more state dependencies than there are state fields.");

    return descriptor;
  }

  private class DependencyAdder : IDependencyDescriptorVisitor
  {
    private readonly Type _actorType;
    private readonly ConstructorDescriptor _descriptor;
    private readonly HashSet<FieldInfo> _addedStateFields;

    public DependencyAdder(Type actorType, ConstructorDescriptor descriptor)
    {
      _actorType = actorType;
      _descriptor = descriptor;
      _addedStateFields = new();
    }

    public void Add(IDependencyDescriptor dependency)
    {
      Debug.Assert(dependency.Index == _descriptor._dependencies.Count, "Wrong index.");

      _descriptor._dependencies.Add(dependency);
      dependency.Accept(this);
    }

    public void VisitContextDependency(IContextDependencyDescriptor dependency)
    {
      if (_descriptor.ContextDependency is not null)
        throw InvalidActorException.DuplicateActorContext(_actorType, dependency.Parameter);

      _descriptor.ContextDependency = dependency;
    }

    public void VisitServiceDependency(IServiceDependencyDescriptor dependency)
    {
      _descriptor._serviceDependencies.Add(dependency);
    }

    public void VisitStateDependency(IStateDependencyDescriptor dependency)
    {
      if (_addedStateFields.Contains(dependency.Field))
        throw InvalidActorException.StateComponentsMismatch(_actorType);

      _descriptor._stateDependencies.Add(dependency);
    }
  }
}
