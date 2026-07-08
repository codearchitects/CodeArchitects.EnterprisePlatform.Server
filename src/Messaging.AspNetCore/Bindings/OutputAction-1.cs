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
  where TMetadata : IOutputMetadata
{
  protected readonly TMetadata _metadata;
  private readonly IServiceProvider _services;

  /// <summary>
  /// Creates a new <see cref="OutputAction{TMetadata}"/>.
  /// </summary>
  /// <param name="metadata">The metadata instance.</param>
  /// <param name="services">The service provider.</param>
  public OutputAction(TMetadata metadata, IServiceProvider services)
  {
    _metadata = metadata;
    _services = services;
  }

  public override bool IsTypeFiltered => false;

  public override bool CanExecute(Type resultType)
  {
    return true;
  }

  public sealed override Task ExecuteAsync<TMessage, TResult>(TMessage message, TResult? result, CancellationToken cancellationToken)
    where TResult : default
  {
    IOutputBinding<TMetadata>? binding = _services.GetService<IOutputBinding<TMetadata>>();
    if (binding is null)
    {
      ILogger logger = _services.CreateMessagingLogger();
      Type outputBindingType = typeof(IOutputBinding<>).MakeGenericType(typeof(TMetadata));
      logger.LogWarning("No output binding was registered for metadata {OutputBindingMetadataType}. Please, register a service as {OutputBindingType}.", typeof(TMetadata), outputBindingType);

      return Task.CompletedTask;
    }

    OutputBindingContext<TMetadata, TMessage, TResult> context = new(_metadata, message, result);
    return binding.ExecuteAsync(context, cancellationToken);
  }
}
