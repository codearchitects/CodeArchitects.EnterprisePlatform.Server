namespace CodeArchitects.Platform.Actors.Scheduling;

public readonly record struct ActivitySpec(string Name, Type? ImplementationType, IReadOnlyList<object?> Arguments)
{
  public ActivitySpec(string Name)
    : this(Name, null, Array.Empty<object?>())
  {
  }

  public ActivitySpec(string Name, Type ImplementationType)
    : this(Name, ImplementationType, Array.Empty<object?>())
  {
  }

  public ActivitySpec(string Name, IReadOnlyList<object?> Arguments)
    : this(Name, null, Arguments)
  {
  }

  public ActivitySpec WithArguments(IReadOnlyList<object?> arguments) => this with { Arguments = arguments };

  public ActivitySpec WithArguments(params object?[] arguments) => this with { Arguments = arguments };

  public ActivitySpec WithImplementationType(Type type) => this with { ImplementationType = type };

  public static implicit operator ActivitySpec(string activityName) => new(activityName);
}
