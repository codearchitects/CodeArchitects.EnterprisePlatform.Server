using CodeArchitects.Platform.Actors.Bindings;
using CodeArchitects.Platform.Actors.Scheduling;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Provides contextual information and operations for the actor instance.
/// </summary>
/// <typeparam name="TActor">The type of actor being managed by this context.</typeparam>
public interface IActorContext<TActor> : IActorContext
  where TActor : class
{
  /// <summary>
  /// Changes the implementation of the current actor instance to the specified implementation.
  /// </summary>
  /// <remarks>
  /// The actor will change implementation after the current execution finishes.
  /// </remarks>
  /// <typeparam name="TImplementation">The type of actor implementation to become.</typeparam>
  void Become<TImplementation>()
    where TImplementation : class, TActor;

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync(Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync(Expression<Action<TActor>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance, specifying the implementation that will execute the activity.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <typeparam name="TImplementation">The type of actor implementation to use for the activity.</typeparam>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync<TImplementation>(Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor;

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance, specifying the implementation that will execute the activity.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <typeparam name="TImplementation">The type of actor implementation to use for the activity.</typeparam>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync<TImplementation>(Expression<Action<TImplementation>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor;

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
  Task ScheduleAsync(ScheduleId id, Expression<Func<TActor, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

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
  Task ScheduleAsync(ScheduleId id, Expression<Action<TActor>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance, specifying the implementation that will execute the activity.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <typeparam name="TImplementation">The type of actor implementation to use for the activity.</typeparam>
  /// <param name="id">An identifier that will be associated to the schedule and that can be used to cancel it.</param>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync<TImplementation>(ScheduleId id, Expression<Func<TImplementation, Task>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor;

  /// <summary>
  /// Schedules an activity to be executed by the current actor instance, specifying the implementation that will execute the activity.
  /// </summary>
  /// <remarks>
  /// The <paramref name="cancellationToken"/> parameter will only affect the scheduling operation, not the execution of the scheduled activity.
  /// </remarks>
  /// <typeparam name="TImplementation">The type of actor implementation to use for the activity.</typeparam>
  /// <param name="id">An identifier that will be associated to the schedule and that can be used to cancel it.</param>
  /// <param name="activity">The activity to execute.</param>
  /// <param name="options">The options for the scheduling operation.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  Task ScheduleAsync<TImplementation>(ScheduleId id, Expression<Action<TImplementation>> activity, SchedulingOptions? options = null, CancellationToken cancellationToken = default)
    where TImplementation : class, TActor;

  /// <summary>
  /// Registers a binding for the current actor instance.
  /// </summary>
  /// <remarks>
  /// This method may only be called inside the actor's constructor.
  /// </remarks>
  /// <param name="configure">
  /// A delegate that configures the binding through an <see cref="IBindingBuilder{TActor}"/>.
  /// </param>
  /// <returns>The id of the registered binding that can be use to enable and disable the binding.</returns>
  BindingId RegisterBinding(Func<IBindingBuilder<TActor>, IBindingResult> configure);

  /// <summary>
  /// Registers a binding for the current actor instance, specifying the implementation that will execute the binding.
  /// </summary>
  /// <remarks>
  /// This method may only be called inside the actor's constructor.
  /// </remarks>
  /// <typeparam name="TImplementation">The type of actor implementation to use for the activity.</typeparam>
  /// <param name="configure">
  /// A delegate that configures the binding through an <see cref="IBindingBuilder{TActor}"/>.
  /// </param>
  /// <returns>The id of the registered binding that can be use to enable and disable the binding.</returns>
  BindingId RegisterBinding<TImplementation>(Func<IBindingBuilder<TImplementation>, IBindingResult> configure)
    where TImplementation : class, TActor;
}
