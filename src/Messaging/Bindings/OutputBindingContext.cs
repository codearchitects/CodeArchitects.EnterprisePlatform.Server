namespace CodeArchitects.Platform.Messaging.Bindings;

/// <summary>
/// Contains data relative to an output binding action.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata configured for the binding.</typeparam>
/// <typeparam name="TMessage">The type of the message handled.</typeparam>
/// <typeparam name="TResult">The type of the result produced by the handling action.</typeparam>
/// <param name="Metadata">The metadata configured for the binding.</param>
/// <param name="Message">The message handled.</param>
/// <param name="Result">The result produced by the handling action.</param>
public record OutputBindingContext<TMetadata, TMessage, TResult>(TMetadata Metadata, TMessage Message, TResult? Result);
