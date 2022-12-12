using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public delegate T Getter<out T>(object instance);

[Experimental]
public delegate void Setter<in T>(object instance, T value);
