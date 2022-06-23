namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

/// <summary>
/// Action that executes after the execution of a message handler.
/// </summary>
internal abstract class OutputAction
{
  /// <summary>
  /// Executes the output action.
  /// </summary>
  /// <typeparam name="TMessage">The type of the handled message.</typeparam>
  /// <typeparam name="TResult">The type of the result.</typeparam>
  /// <param name="message">The message instance.</param>
  /// <param name="result">The result</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns></returns>
  public abstract Task ExecuteAsync<TMessage, TResult>(TMessage message, TResult? result, CancellationToken cancellationToken)
    where TMessage : class
    where TResult : class;

  /// <summary>
  /// Creates a new <see cref="OutputAction{TMetadata}"/> where <c>TMetadata</c> is <paramref name="metadataType"/>.
  /// </summary>
  /// <param name="metadataType">The type of the metadata.</param>
  /// <param name="metadata">The metadata instance.</param>
  /// <param name="services">The service provider.</param>
  /// <returns>The created instance.</returns>
  /// <exception cref="ArgumentException">Thrown when <paramref name="metadata"/> is not assignable to <paramref name="metadataType"/>.</exception>
  public static OutputAction Create(Type metadataType, object metadata, IServiceProvider services)
  {
    return metadataType.IsInstanceOfType(metadata)
      ? (OutputAction)Activator.CreateInstance(typeof(OutputAction<>).MakeGenericType(metadataType), new[] { services, metadata })!
      : throw new ArgumentException($"Value of '{nameof(metadata)}' is not assignable to type {metadataType.Name}.", nameof(metadata));
  }
}
