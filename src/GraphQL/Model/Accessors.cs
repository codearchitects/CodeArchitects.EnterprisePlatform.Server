namespace CodeArchitects.Platform.GraphQL.Model;

internal delegate T Getter<out T>(object instance);

internal delegate void Setter<in T>(object instance, T value);
