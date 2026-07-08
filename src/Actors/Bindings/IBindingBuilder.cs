namespace CodeArchitects.Platform.Actors.Bindings;

/// <summary>
/// Represents a builder used for configuring a binding.
/// </summary>
/// <typeparam name="TActor">The type of the actor defining the binding.</typeparam>
public interface IBindingBuilder<TActor>
  where TActor : class
{
  /// <summary>
  /// Specifies the pre-condition that must be met before any actor activity is executed.
  /// </summary>
  /// <param name="preCondition">A function that determines if the activity should be executed or not.</param>
  /// <returns>The same builder.</returns>
  IBindingBuilder<TActor> WithPreCondition(Predicate<TActor> preCondition);

  /// <summary>
  /// Specifies the post-condition that must be met after any actor activity is executed.
  /// </summary>
  /// <param name="postCondition">A function that determines if the activity should be executed or not.</param>
  /// <returns>The same builder.</returns>
  IBindingBuilder<TActor> WithPostCondition(Predicate<TActor> postCondition);

  /// <summary>
  /// Enables or disables the binding.
  /// </summary>
  /// <param name="enabled"><see langword="true"/> to enable the binding, <see langword="false"/> to disable it.</param>
  /// <returns>The same builder.</returns>
  IBindingBuilder<TActor> IsEnabled(bool enabled = true);

  /// <summary>
  /// Specifies the activity to execute if the pre-condition and the post-conditions are met.
  /// </summary>
  /// <param name="activity">The activity to execute.</param>
  /// <returns>The result of the binding.</returns>
  IBindingResult BindTo(Func<TActor, CancellationToken, Task> activity);

  /// <summary>
  /// Specifies the activity to execute if the pre-condition and the post-conditions are met.
  /// </summary>
  /// <param name="activity">The activity to execute.</param>
  /// <returns>The result of the binding.</returns>
  IBindingResult BindTo(Action<TActor> activity);
}
