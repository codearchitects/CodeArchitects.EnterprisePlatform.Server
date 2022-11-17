using CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

public static class MultitenancyDataOptionsBuilderExtensions
{
  public static DataOptionsBuilder UseMultitenancy(this DataOptionsBuilder builder, IMultitenancyDescriptor descriptor)
  {
    ArgumentNullException.ThrowIfNull(builder);
    ArgumentNullException.ThrowIfNull(descriptor);

    return builder.UseMultitenancyCore(descriptor);
  }

  public static DataOptionsBuilder UseMultitenancy(this DataOptionsBuilder builder, bool disableModificationFilters = false)
  {
    ArgumentNullException.ThrowIfNull(builder);

    return builder.UseMultitenancyCore(new DefaultMultitenancyDescriptor(typeof(Guid), disableModificationFilters));
  }

  private static DataOptionsBuilder UseMultitenancyCore(this DataOptionsBuilder builder, IMultitenancyDescriptor descriptor)
  {
    if (descriptor.MultitenancyContextType is null && descriptor.MultitenancyContextImplementationFactory is null)
      throw new ArgumentException($"The descriptor must specify a {nameof(IMultitenancyDescriptor)} type or factory.", nameof(descriptor));
    if (descriptor.TenantIdType is null)
      throw new ArgumentException("The descriptor must specify a tenant id type.", nameof(descriptor));
    if (!descriptor.TenantIdType.IsAssignableTo(typeof(IEquatable<>).MakeGenericType(descriptor.TenantIdType)))
      throw new ArgumentException($"The tenant id type must implement the generic '{typeof(IEquatable<>).Name}' interface.", nameof(descriptor));

    (builder as IDataOptionsBuilderInfrastructure).AddOrUpdatePlugin(new MultitenancyDataPlugin(descriptor));
    return builder;
  }
}
