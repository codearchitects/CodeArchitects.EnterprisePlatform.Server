// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETCOREAPP3_1_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute))]
#else

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that a method that will never return under any circumstance.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[ExcludeFromCodeCoverage, DebuggerNonUserCode]
internal sealed class DoesNotReturnAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the <see cref="DoesNotReturnAttribute"/> class.
  /// </summary>
  public DoesNotReturnAttribute() { }
}

#endif
