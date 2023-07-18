namespace CodeArchitects.Platform.GraphQL.Model;

public delegate T Getter<out T>(object instance);

public delegate void Setter<in T>(object instance, T value);
