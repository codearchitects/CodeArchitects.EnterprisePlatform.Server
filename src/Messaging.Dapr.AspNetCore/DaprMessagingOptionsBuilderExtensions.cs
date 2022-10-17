namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for <see cref="IDaprMessagingOptionsBuilder"/>.
/// </summary>
public static class DaprMessagingOptionsBuilderExtensions
{
  /// <summary>
  /// Registers an handler using its attributes.
  /// </summary>
  /// <param name="builder">The builder.</param>
  /// <typeparam name="THandler">The handler type.</typeparam>
  /// <returns>The builder.</returns>
  public static IDaprMessagingOptionsBuilder AddHandler<THandler>(this IDaprMessagingOptionsBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.AddHandler(typeof(THandler));
  }

  /// <summary>
  /// Registers a message type.
  /// </summary>
  /// <param name="builder">The builder.</param>
  /// <typeparam name="TMessage">The message type.</typeparam>
  /// <returns>The builder.</returns>
  public static IDaprMessagingOptionsBuilder AddMessage<TMessage>(this IDaprMessagingOptionsBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.AddMessage(typeof(TMessage));
  }

  /// <summary>
  /// Scans the assembly of the given type for registering messages and message handlers.
  /// </summary>
  /// <param name="builder">The builder.</param>
  /// <typeparam name="T">A marker type of the assembly.</typeparam>
  /// <returns>The builder.</returns>
  public static IDaprMessagingOptionsBuilder ScanAssemblyOfType<T>(this IDaprMessagingOptionsBuilder builder)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.ScanAssembly(typeof(T).Assembly);
  }

  /// <summary>
  /// Registers an output binding to the application services with the given lifetime.
  /// </summary>
  /// <typeparam name="TOutputBinding">The output binding type.</typeparam>
  /// <param name="builder">The builder.</param>
  /// <param name="lifetime">The service lifetime.</param>
  /// <returns>The builder.</returns>
  public static IDaprMessagingOptionsBuilder RegisterOutputBinding<TOutputBinding>(this IDaprMessagingOptionsBuilder builder, ServiceLifetime lifetime = ServiceLifetime.Scoped)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.RegisterOutputBinding(typeof(TOutputBinding), lifetime);
  }

  /// <summary>
  /// Registers an alias for an output metadata type that can be used instead of its fully-qualified name in configuration, prefixing it with '$'.
  /// </summary>
  /// <typeparam name="TMetadata">The metadata type.</typeparam>
  /// <param name="builder">The builder.</param>
  /// <param name="alias">The alias.</param>
  /// <returns>The builder.</returns>
  public static IDaprMessagingOptionsBuilder RegisterOutputMetadataAlias<TMetadata>(this IDaprMessagingOptionsBuilder builder, string alias)
  {
    if (builder is null)
      throw new ArgumentNullException(nameof(builder));

    return builder.RegisterOutputMetadataAlias(alias, typeof(TMetadata));
  }
}
