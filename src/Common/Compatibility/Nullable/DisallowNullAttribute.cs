namespace System.Diagnostics.CodeAnalysis;

#if !NETCOREAPP3_0_OR_GREATER

/// <summary>
/// Specifies that <see langword="null"/> is disallowed as an input even if the corresponding type allows it.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
[ExcludeFromCodeCoverage]
internal sealed class DisallowNullAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DisallowNullAttribute"/> class.
  /// </summary>
  public DisallowNullAttribute()
  {
  }
}

#endif
