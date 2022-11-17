namespace System.Diagnostics.CodeAnalysis;

#if !NETCOREAPP3_0_OR_GREATER

/// <summary>
/// Specifies that a method that will never return under any circumstance.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[ExcludeFromCodeCoverage]
internal sealed class DoesNotReturnAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DoesNotReturnAttribute"/> class.
  /// </summary>
  public DoesNotReturnAttribute() { }
}

#endif
