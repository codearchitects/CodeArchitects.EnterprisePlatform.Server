extern alias CaPlatformCommon;
using CaPlatformCommon.CodeArchitects.Platform.Common.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

/// <summary>
/// A delegate that can be used to get the value of a member on an object instance.
/// </summary>
/// <typeparam name="T">The type of the value to get.</typeparam>
/// <param name="instance">The instance of the object.</param>
/// <returns>The value of the member.</returns>
[Experimental]
public delegate T Getter<out T>(object instance);

/// <summary>
/// A delegate that can be used to set the value of a member on an object instance.
/// </summary>
/// <typeparam name="T">The type of the value to set.</typeparam>
/// <param name="instance">The instance of the object.</param>
/// <param name="value">The value to set for the member.</param>
[Experimental]
public delegate void Setter<in T>(object instance, T value);
