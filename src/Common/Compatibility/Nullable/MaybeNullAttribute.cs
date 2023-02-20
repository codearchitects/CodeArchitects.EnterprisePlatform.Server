// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETCOREAPP3_1_OR_GREATER
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Diagnostics.CodeAnalysis.MaybeNullAttribute))]
#else

namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that an output may be <see langword="null"/> even if the corresponding type disallows it.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
[ExcludeFromCodeCoverage, DebuggerNonUserCode]
internal sealed class MaybeNullAttribute : Attribute
{
  /// <summary>
  /// Initializes a new instance of the <see cref="MaybeNullAttribute"/> class.
  /// </summary>
  public MaybeNullAttribute() { }
}

#endif
