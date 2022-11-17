namespace System.Diagnostics.CodeAnalysis;

#if !NETCOREAPP3_0_OR_GREATER

/// <summary>
/// Specifies that <see langword="null"/> is allowed as an input even if the corresponding type disallows it.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
[ExcludeFromCodeCoverage]
internal sealed class AllowNullAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DisallowNullAttribute"/> class.
  /// </summary>
  public AllowNullAttribute()
  {
  }
}

#endif
