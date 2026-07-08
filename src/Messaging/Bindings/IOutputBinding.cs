namespace CodeArchitects.Platform.Messaging.Bindings;

/// <summary>
/// Injects behavior after a message handling action.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata that are relative to the binding.</typeparam>
public interface IOutputBinding<TMetadata>
  where TMetadata : IOutputMetadata
{
  /// <summary>
  /// Action that executes after a message handling action.
  /// </summary>
  /// <typeparam name="TMessage">The type of the message handled.</typeparam>
  /// <typeparam name="TResult">The type of the result of the handling action.</typeparam>
  /// <param name="context">An object containing data relative to this action.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns></returns>
  Task ExecuteAsync<TMessage, TResult>(OutputBindingContext<TMetadata, TMessage, TResult> context, CancellationToken cancellationToken);
}
