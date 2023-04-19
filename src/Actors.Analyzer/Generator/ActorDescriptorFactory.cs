using CodeArchitects.Platform.Actors.Analyzer.Utils;
using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal readonly ref partial struct ActorDescriptorFactory
{
  private readonly bool _disableActorFactoryGeneration;
  private readonly bool _disableActorDiagnostics;
  private readonly ICollection<ActorDescriptor> _descriptors;
  private readonly ICollection<DiagnosticReference> _diagnostics;
  private readonly Dictionary<ITypeSymbol, ActorEntry> _entries;
  private readonly CancellationToken _cancellationToken;

  public ActorDescriptorFactory(ActorAnalyzerOptions options, CancellationToken cancellationToken)
  {
    _disableActorFactoryGeneration = options.ShouldDisableActorFactoryGeneration();
    _disableActorDiagnostics = options.ShouldDisableActorDiagnostics();

    _descriptors = _disableActorFactoryGeneration
      ? Array.Empty<ActorDescriptor>()
      : new List<ActorDescriptor>();

    _diagnostics = _disableActorDiagnostics
      ? Array.Empty<DiagnosticReference>()
      : new List<DiagnosticReference>();

    _entries = new(SymbolEqualityComparer.Default);
    _cancellationToken = cancellationToken;
  }

  public DescriptorResult GetResult()
  {
    foreach (KeyValuePair<ITypeSymbol, ActorEntry> pair in _entries)
    {
      _cancellationToken.ThrowIfCancellationRequested();

      ActorEntry entry = pair.Value;
      Analyze(pair.Key, entry.Actor, entry.Factories, entry.Implementations);
    }

    return new(
      Descriptors: new RecordList<ActorDescriptor>((IReadOnlyList<ActorDescriptor>)_descriptors),
      Diagnostics: new RecordList<DiagnosticReference>((IReadOnlyList<DiagnosticReference>)_diagnostics));
  }

  public void AddActor(ActorData actor)
  {
    INamedTypeSymbol actorType = actor.ActorType;

    _ = _entries.TryGetValue(actorType, out ActorEntry entry);
    _entries[actorType] = entry with { Actor = actor };
  }

  public void AddImplementation(ImplementationData implementation)
  {
    if (!SymbolExtensions.TryGetAttributeTargetType(implementation.ImplementationAttribute, implementation.GenericImplementationAttribute, out ITypeSymbol? targetType))
      return;

    _ = _entries.TryGetValue(targetType, out ActorEntry entry);

    List<ImplementationData>? implementations = entry.Implementations;
    if (implementations is null)
    {
      implementations = new();
      _entries[targetType] = entry with { Implementations = implementations };
    }

    implementations.Add(implementation);
  }

  public void AddFactory(FactoryData factory)
  {
    if (!SymbolExtensions.TryGetAttributeTargetType(factory.FactoryAttribute, factory.GenericFactoryAttribute, out ITypeSymbol? targetType))
      return;

    _ = _entries.TryGetValue(targetType, out ActorEntry entry);

    List<FactoryData>? factories = entry.Factories;
    if (factories is null)
    {
      factories = new();
      _entries[targetType] = entry with { Factories = factories };
    }

    factories.Add(factory);
  }

  private void AddDescriptor(INamedTypeSymbol actorType, INamedTypeSymbol interfaceType, ITypeSymbol? idType, bool isVirtual, bool getsIdFromState, List<StateDependencyDescriptor> stateDependencies)
  {
    if (_disableActorFactoryGeneration)
      return;

    ActorDescriptor descriptor = new(
      Namespace: actorType.ContainingNamespace.ToDisplayString(Format.FullName),
      ImplementationTypeName: actorType.Name,
      InterfaceTypeFullName: interfaceType.ToDisplayString(Format.GlobalFullName),
      IdTypeFullName: idType?.ToDisplayString(Format.GlobalFullName) ?? "string",
      IsVirtual: isVirtual,
      GetsIdFromState: getsIdFromState,
      StateDependencies: new RecordList<StateDependencyDescriptor>(stateDependencies));

    _descriptors.Add(descriptor);
  }

  private readonly record struct ActorEntry(
    ActorData? Actor,
    List<FactoryData>? Factories,
    List<ImplementationData>? Implementations);
}
