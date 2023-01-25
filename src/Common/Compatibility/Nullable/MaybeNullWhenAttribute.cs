#if NETCOREAPP3_1_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute))]
#else

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be <see langword="null"/> even if the corresponding type disallows it.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
[ExcludeFromCodeCoverage, DebuggerNonUserCode]
internal sealed class MaybeNullWhenAttribute : Attribute
{
  /// <summary>
  /// Initializes the attribute with the specified return value condition.
  /// </summary>
  /// <param name="returnValue">
  /// Gets the return value condition.
  /// If the method returns this value, the associated parameter may be <see langword="null"/>.
  /// </param>
  public MaybeNullWhenAttribute(bool returnValue)
  {
    ReturnValue = returnValue;
  }

  /// <summary>
  /// Gets the return value condition.
  /// If the method returns this value, the associated parameter may be <see langword="null"/>.
  /// </summary>
  public bool ReturnValue { get; }
}

#endif
