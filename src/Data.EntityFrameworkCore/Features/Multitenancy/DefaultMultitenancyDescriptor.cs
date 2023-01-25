using CodeArchitects.Platform.Data.Features.Multitenancy;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

/// <summary>
/// Default implementation of <see cref="IMultitenancyDescriptor"/>.
/// </summary>
/// <typeparam name="TTenantId">The type of the tenant identifier.</typeparam>
public class DefaultMultitenancyDescriptor<TTenantId> : IMultitenancyDescriptor
  where TTenantId : IEquatable<TTenantId>
{
  /// <summary>
  /// Creates a new <see cref="DefaultMultitenancyDescriptor{TTenantId}"/>.
  /// </summary>
  public DefaultMultitenancyDescriptor()
  {
  }

  /// <summary>
  /// Creates a new <see cref="DefaultMultitenancyDescriptor{TTenantId}"/>, specifying whether the modification interceptors should be enabled.
  /// </summary>
  /// <param name="usesModificationInterceptors">Specifies whether the modification interceptors should be enabled.</param>
  public DefaultMultitenancyDescriptor(bool usesModificationInterceptors)
  {
    UsesModificationInterceptors = usesModificationInterceptors;
  }

  /// <inheritdoc/>
  public virtual Type TenantIdType => typeof(TTenantId);

  /// <inheritdoc/>
  public virtual Type? MultitenancyContextType => typeof(ProfileMultitenancyContext<TTenantId>);

  /// <inheritdoc/>
  public virtual Func<IServiceProvider, IMultitenancyContext>? MultitenancyContextImplementationFactory => null;

  /// <inheritdoc/>
  public virtual bool UsesModificationInterceptors { get; }
}
