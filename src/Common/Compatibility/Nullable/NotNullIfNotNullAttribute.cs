#if NETCOREAPP3_1_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute))]
#else

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that the output will be non-<see langword="null"/> if the named parameter is non-<see langword="null"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
internal sealed class NotNullIfNotNullAttribute : Attribute
{
  /// <summary>
  /// Initializes the attribute with the associated parameter name.
  /// </summary>
  /// <param name="parameterName">
  /// The associated parameter name.
  /// The output will be non-<see langword="null"/> if the argument to the parameter specified is non-<see langword="null"/>.
  /// </param>
  public NotNullIfNotNullAttribute(string parameterName)
  {
    ParameterName = parameterName;
  }

  /// <summary>
  /// Gets the associated parameter name.
  /// The output will be non-<see langword="null"/> if the argument to the parameter specified is non-<see langword="null"/>.
  /// </summary>
  public string ParameterName { get; }
}

#endif
