using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal abstract class TypeData : IEquatable<TypeData>
{
  protected abstract INamedTypeSymbol Type { get; }

  public abstract void AddTo(in ActorDescriptorFactory factory);

  public bool Equals(TypeData? other) => other is not null && SymbolEqualityComparer.Default.Equals(Type, other.Type);

  public override bool Equals(object? obj) => Equals(obj as TypeData);

  public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(Type);
}
