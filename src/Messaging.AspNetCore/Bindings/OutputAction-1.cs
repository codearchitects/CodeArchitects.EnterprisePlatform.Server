using CodeArchitects.Platform.Messaging.AspNetCore.Utils;
using CodeArchitects.Platform.Messaging.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

/// <summary>
/// Generic implementation of <see cref="OutputAction"/>.
/// </summary>
/// <typeparam name="TMetadata">The type of the metadata.</typeparam>
internal class OutputAction<TMetadata> : OutputAction
{
  private readonly IServiceProvider _services;
  private readonly TMetadata _metadata;

  /// <summary>
  /// Creates a new <see cref="OutputAction{TMetadata}"/>.
  /// </summary>
  /// <param name="services">The service provider.</param>
  /// <param name="metadata">The metadata instance.</param>
  public OutputAction(IServiceProvider services, TMetadata metadata)
  {
    _services = services;
    _metadata = metadata;
  }

  public override Task ExecuteAsync<TMessage, TResult>(TMessage message, TResult? result, CancellationToken cancellationToken)
    where TMessage : class
    where TResult : class
  {
    IOutputBinding<TMetadata>? binding = _services.GetService<IOutputBinding<TMetadata>>();
    if (binding is null)
    {
      if (_services.TryGetLogger() is ILogger logger)
      {
        logger.LogWarning("No output binding was registered for metadata {OutputBindingMetadataType}. Please, register a service as IOutputBinding<{OutputBindingMetadataType}>.", typeof(TMetadata));
      }
      return Task.CompletedTask;
    }

    OutputBindingContext<TMetadata, TMessage, TResult> context = new(_metadata, message, result);
    return binding.ExecuteAsync(context, cancellationToken);
  }
}
