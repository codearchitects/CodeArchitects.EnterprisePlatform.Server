namespace CodeArchitects.Platform.Data.AdoNet.Model;

public delegate T Getter<out T>(object instance);

public delegate void Setter<in T>(object instance, T value);
