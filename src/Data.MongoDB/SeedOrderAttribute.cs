using CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.MongoDB;

/// <summary>
/// Declares the order in which a <see cref="DataSeed"/> is applied, lowest first.
/// </summary>
/// <remarks>
/// Seeds without the attribute are ordered as if they declared <c>0</c>. Ties are broken by the
/// full name of the type, so the sequence is deterministic even when no order is declared —
/// neither assembly scanning nor service resolution guarantees a stable order by itself.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="SeedOrderAttribute"/> class.
/// </remarks>
/// <param name="order">The order, lowest first.</param>
[Experimental]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SeedOrderAttribute(int order) : Attribute
{

  /// <summary>
  /// The order in which the seed is applied, lowest first.
  /// </summary>
  public int Order { get; } = order;
}
