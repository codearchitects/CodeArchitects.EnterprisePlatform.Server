using CodeArchitects.Platform.Data.Features.Multitenancy;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

/// <summary>
/// Describes the multitenancy feature of the CAEP extension.
/// </summary>
public interface IMultitenancyDescriptor
{
  /// <summary>
  /// If <see langword="true"/> the modification interceptors for multitenancy will be enabled.
  /// </summary>
  bool UsesModificationInterceptors { get; }

  /// <summary>
  /// The type of the tenant identifier.
  /// </summary>
  Type TenantIdType { get; }

  /// <summary>
  /// The implementation type of <see cref="IMultitenancyContext"/>.
  /// </summary>
  Type? MultitenancyContextType { get; }

  /// <summary>
  /// The implementation factory of <see cref="IMultitenancyContext"/>.
  /// </summary>
  Func<IServiceProvider, IMultitenancyContext>? MultitenancyContextImplementationFactory { get; }
}
