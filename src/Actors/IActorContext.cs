using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Scheduling;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Provides contextual information and operations for the actor instance.
/// </summary>
public interface IActorContext
{
  /// <summary>
  /// Gets the identifier of the current actor instance.
  /// </summary>
  string ActorId { get; }

  /// <summary>
  /// Changes the implementation of the current actor instance to the specified implementation.
  /// </summary>
  /// <remarks>
  /// The actor will change implementation after the current execution finishes.
  /// </remarks>
  /// <param name="implementationType">The type of actor implementation to become.</param>
  void Become(Type implementationType);

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync(ActivitySpec activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <param name="id">An identifier that will be associated to the schedule and that can be used to cancel it.</param>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync(ScheduleId id, ActivitySpec activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Unschedules a previously scheduled activity with the given id.
  /// </summary>
  /// <param name="scheduleId">The unique identifier of the scheduled activity to unscheduled.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task UnscheduleAsync(ScheduleId scheduleId, CancellationToken cancellationToken = default);
  
  /// <summary>
  /// Enables a previously registered binding.
  /// </summary>
  /// <param name="id">The id of the binding to enable.</param>
  /// <remarks>
  /// When a binding is enabled, it will be executed automatically whenever its pre-condition and post-condition are met. If the binding was already enabled, calling this method has no effect.
  /// </remarks>
  void EnableBinding(BindingId id);

  /// <summary>
  /// Disables a previously registered binding.
  /// </summary>
  /// <param name="id">The id of the binding to disable.</param>
  /// <remarks>
  /// When a binding is disabled, it will not be executed automatically, even if its pre-condition and post-condition are met. If the binding was already disabled, calling this method has no effect.
  /// </remarks>
  void DisableBinding(BindingId id);
}
