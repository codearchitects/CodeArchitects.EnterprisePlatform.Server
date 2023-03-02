namespace CodeArchitects.Platform.Actors.Scheduling;

/// <summary>
/// Describes an activity that can be schedule.
/// </summary>
/// <param name="Name">The name of the method to execute.</param>
/// <param name="ImplementationType">The type of actor implementation to use for the activity.</param>
/// <param name="Arguments">The arguments to pass to the activity method.</param>
public readonly record struct ActivitySpec(string Name, Type? ImplementationType, IReadOnlyList<object?> Arguments)
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ActivitySpec"/> class with the specified name and no arguments, without specifying an implementation type.
  /// </summary>
  /// <param name="Name">The name of the method to execute.</param>
  public ActivitySpec(string Name)
    : this(Name, null, Array.Empty<object?>())
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ActivitySpec"/> class with the specified name and no arguments, specifying an implementation type.
  /// </summary>
  /// <param name="Name">The name of the method to execute.</param>
  /// <param name="ImplementationType">The type of actor implementation to use for the activity.</param>
  public ActivitySpec(string Name, Type ImplementationType)
    : this(Name, ImplementationType, Array.Empty<object?>())
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ActivitySpec"/> class with the specified name and arguments, without specifying an implementation type.
  /// </summary>
  /// <param name="Name">The name of the method to execute.</param>
  /// <param name="Arguments">The arguments to pass to the activity method.</param>
  public ActivitySpec(string Name, IReadOnlyList<object?> Arguments)
    : this(Name, null, Arguments)
  {
  }

  /// <summary>
  /// Specifies the arguments of the activity method.
  /// </summary>
  /// <param name="arguments">The arguments to pass to the activity.</param>
  /// <returns>A new instance of the <see cref="ActivitySpec"/> class with the specified arguments.</returns>
  public ActivitySpec WithArguments(IReadOnlyList<object?> arguments) => this with { Arguments = arguments };

  /// <summary>
  /// Specifies the arguments of the activity method.
  /// </summary>
  /// <param name="arguments">The arguments to pass to the activity.</param>
  /// <returns>A new instance of the <see cref="ActivitySpec"/> class with the specified arguments.</returns>
  public ActivitySpec WithArguments(params object?[] arguments) => this with { Arguments = arguments };

  /// <summary>
  /// Specifies the implementation type to use for the activity.
  /// </summary>
  /// <param name="implementationType">The type of actor implementation to use for the activity.</param>
  /// <returns></returns>
  public ActivitySpec WithImplementationType(Type implementationType) => this with { ImplementationType = implementationType };

  /// <summary>
  /// Implicitly converts a string representing the method name to an <see cref="ActivitySpec"/>.
  /// </summary>
  /// <param name="activityName"></param>
  public static implicit operator ActivitySpec(string activityName) => new(activityName);
}
