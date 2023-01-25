#if NETCOREAPP3_1_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute))]
#else

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that the method will not return if the associated <see cref="bool"/> parameter is passed the specified value.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
[ExcludeFromCodeCoverage, DebuggerNonUserCode]
internal sealed class DoesNotReturnIfAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DoesNotReturnIfAttribute"/> class with the specified parameter value.
  /// </summary>
  /// <param name="parameterValue">
  /// The condition parameter value.
  /// Code after the method is considered unreachable by diagnostics if the argument to the associated parameter matches this value.
  /// </param>
  public DoesNotReturnIfAttribute(bool parameterValue)
  {
    ParameterValue = parameterValue;
  }

  /// <summary>
  /// Gets the condition parameter value.
  /// Code after the method is considered unreachable by diagnostics if the argument to the associated parameter matches this value.
  /// </summary>
  public bool ParameterValue { get; }
}

#endif
